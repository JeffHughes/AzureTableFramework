import { ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import {
  CardSettingsModel,
  DialogSettingsModel,
  SortSettingsModel,
} from '@syncfusion/ej2-angular-kanban';
import { User } from '../login/login.component';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'app-kanban',
  templateUrl: './kanban.component.html',
  styleUrls: ['./kanban.component.scss'],
})
export class KanbanComponent {
  constructor(private db: AngularFirestore) {
    this.sub();
  }
  @ViewChild('kb') kb;
  @ViewChild('spreadsheet') spreadsheet;

  public data = [];

  showCurve = false;
  updateFlag = false;

  highcharts = Highcharts;
  chartOptions = {
    title: {
      text: '',
      style: {
        display: 'none',
      },
    },
    subtitle: {
      text: '',
      style: {
        display: 'none',
      },
    },
    chart: {
      type: 'areaspline',
      zoomType: 'x',
      backgroundColor: null,
      height: '50%',
    },

    xAxis: {
      categories: ['Always', 'Often', 'Meets', 'Below'],
      visible: false,
    },

    yAxis: {
      visible: false,
      reversed: true,
    },

    tooltip: {
      pointFormat:
        '{series.name} {point.label} has <b>{point.y:,.1f}% </b> of employees',
    },
    plotOptions: {
      area: {
        startPoint: 'Always',
        marker: {
          enabled: false,
          symbol: 'circle',
          radius: 2,
          states: {
            hover: {
              enabled: true,
            },
          },
        },
      },
    },
    series: [
      {
        name: 'Ideal',
        data: [20, 35, 40, 5],
      },
      {
        name: 'Current',
        data: [10, 10, 10, 10],
      },
    ],
    credits: {
      enabled: false,
    },
    legend: { enabled: false },
  };

  public sortSettings: SortSettingsModel = {
    field: 'RankId',
  };
  public cardSettings: CardSettingsModel = {
    contentField: 'Notes',
    headerField: 'Id',
  };
  public dialogSettings: DialogSettingsModel = {
    fields: [
      { key: 'Name', type: 'TextBox' },
      { key: 'Area', type: 'DropDown' },
      { key: 'Role', type: 'TextBox' },
      { key: 'Level', type: 'TextBox' },
      { key: 'Notes', type: 'TextArea' },
      { key: 'Promote', type: 'Numeric' },
      { key: 'Flag', type: 'Numeric' },
    ],
  };

  employeeFBDoc;
  employeeFBDocWatcher;

  viewable = false;
  isAdmin = false;
  isUser = false;
  showSpreadsheet = false;

  loading = true;

  lastSavedData;

  actionCount = 0;
  dataChanged = true;

  keys = ['Always', 'Often', 'Meets', 'Below'];

  updateChartDataTimeout: any = null;
  sub(): void {
    this.employeeFBDoc = this.db
      .collection('employees')
      .doc('G4ESm6jZpjhkJ0Hc0WnU');

    this.employeeFBDocWatcher = this.employeeFBDoc.valueChanges();

    this.employeeFBDocWatcher.subscribe((dataDoc) => {
      if (
        dataDoc.data &&
        JSON.stringify(this.data) !== JSON.stringify(dataDoc.data)
      ) {
        this.loading = false;

        this.data = this.SortEmployees(dataDoc.data);
        this.lastSavedData = JSON.parse(JSON.stringify(this.data));
        const user: User = JSON.parse(localStorage.getItem('user'));

        const username = user.email
          .toLowerCase()
          .replace('@tradestation.com', '');

        this.isAdmin = false;
        this.viewable = false;
        this.isUser = false;

        dataDoc.roles.forEach((role) => {
          const s = role.split(':');
          const u = s[0];
          const r = s[1];

          if (u === username) {
            switch (r) {
              case 'Admin':
                this.isAdmin = true;
                this.viewable = true;
                break;

              case 'User':
                this.isUser = true;
                this.isAdmin = false;
                this.viewable = true;
                break;
            }
          }
        });
      }
    });
  }

  addCard(): void {
    const id = this.data.length + 1;

    const card = {
      Id: id,
      Area: 'Meets',
      Notes: '',
      Promote: 0,
      Flag: 0,
    };

    this.kb.addCard(card);
    this.kb.openDialog('Edit', card);
  }

  getClass(data): string {
    let classes = 'e-card-content ';

    if (data.Flag) {
      classes += 'Flag Flag' + data.Flag;
    }

    return classes;
  }

  saveBoard(): void {
    console.log('saved');

    this.data.forEach((d) => {
      d.Promote ??= 0;
      d.Flag ??= 0;
      d.Role ??= 'NA';
      d.Level ??= 'NA';

      if (isNaN(d.RankId)) {
        d.RankId = this.data.length + 1;
      }
    });

    this.employeeFBDoc.set(
      {
        data: this.data,
        backup: null,
      },
      { merge: true }
    );
    this.lastSavedData = JSON.parse(JSON.stringify(this.data));

    this.reset();
  }

  getTotal(text): string {
    const area = this.data.filter((f) => f.Area === text);

    return (
      text +
      ' ' +
      area.length +
      // '/' +
      // this.data.length +
      ' (' +
      ((100 * area.length) / this.data.length).toFixed(1) +
      '%)'
    );
  }

  getIdeal(num): string {
    const percent = this.chartOptions.series[0].data[num].toFixed(1);
    const roundNum = Math.floor(+percent * this.data.length / 100);
    return   roundNum + ' (' + percent + '%)';
  }

  actionComplete(): void {
    setTimeout(() => {
      this.data = this.SortEmployees(this.kb.dataSource);
      this.kb.dataSource = this.data;

      try {
        this.spreadsheet ??= {};
        this.spreadsheet.dataSource = this.data;
      } catch (err) {
        console.error(err);
      }
    }, 100);
    const existingIDs = [];
    let counter = 1;
    this.data.forEach((d) => {
      while (existingIDs.includes(d.Id)) {
        d.Id = this.data.length + counter++;
      }
      existingIDs.push(d.Id);
    });

    this.dataChanged = true;
    this.actionCount++;
  }

  private SortEmployees(source): any[] {
    const employeeAreas = {};
    source.forEach((employee) => {
      if (!employeeAreas[employee.Area]) {
        employeeAreas[employee.Area] = [];
      }
      if (!employeeAreas[employee.Area].some((e) => e.Name === employee.Name)) {
        employeeAreas[employee.Area].push(employee);
      }
    });

    const employees = [];
    let OverAllRankCounter = 1;

    this.keys.forEach((k) => {
      if (employeeAreas[k] && employeeAreas[k].length > 0) {
        employeeAreas[k].sort((a, b) => (a.RankId > b.RankId ? 1 : -1));
        let counter = 1;
        employeeAreas[k].forEach((e) => {
          e.RankId = counter++;
          e.OverAllRank = OverAllRankCounter++;
          employees.push(this.sortKeys(e));
        });
      }
    });
    return employees;
  }

  updateChartData(): void {
    if (this.data.length > 4) {
      clearTimeout(this.updateChartDataTimeout);

      this.updateChartDataTimeout = setTimeout(() => {
        const seriesData = [];

        this.keys.forEach((k) => {
          const areaEmployees = this.data.filter((f) => f.Area === k);

          const percentageOfTotal =
            (100 * areaEmployees.length) / this.data.length;
          if (percentageOfTotal >= 0 && percentageOfTotal <= 100) {
            seriesData.push(percentageOfTotal);
          } else {
            seriesData.push(0);
          }
        });

        if (seriesData.length === 4 && seriesData.reduce((a, b) => a + b) > 1) {
          this.chartOptions.series[1].data = seriesData;
        }

        this.updateFlag = true;
        this.showCurve = true;
      }, 100);
    }
  }

  sortKeys(item): object {
    return {
      OverAllRank: item.OverAllRank,
      Name: item.Name,
      Area: item.Area,
      RankId: item.RankId,
      Role: item.Role,
      Level: item.Level,
      Promote: item.Promote,
      Flag: item.Flag,
      Notes: item.Notes,
      Id: item.Id,
    };
  }

  reset(): void {
    this.kb.dataSource = this.lastSavedData;
    try {
      this.spreadsheet ??= {};
      this.spreadsheet.dataSource = this.lastSavedData;
    } catch (err) {
      console.error(err);
    }
    this.data = JSON.parse(JSON.stringify(this.lastSavedData));
    this.actionCount = 0;
    this.dataChanged = true;

    this.showCurve = false;
  }

  beforeSave(args): void {
    args.isFullPost = false;
    args.needBlobData = true;

    console.log(args);

    console.log(this.spreadsheet);
  }

  saveComplete(args): void {
    console.log(args);

    alert("haven't implemented save yet - copy paste to excel");
  }
}
