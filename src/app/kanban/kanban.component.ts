import { ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { AngularFirestore } from '@angular/fire/firestore';
import {
  CardSettingsModel,
  DialogSettingsModel,
  SortSettingsModel,
} from '@syncfusion/ej2-angular-kanban';
import { User } from '../login/login.component';

@Component({
  selector: 'app-kanban',
  templateUrl: './kanban.component.html',
  styleUrls: ['./kanban.component.scss'],
})
export class KanbanComponent implements OnInit {
  constructor(private db: AngularFirestore) {
    this.sub();
  }
  @ViewChild('kb') kb;
  @ViewChild('spreadsheet') spreadsheet;

  public data = [];

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
  sub() {
    this.employeeFBDoc = this.db
      .collection('employees')
      .doc('G4ESm6jZpjhkJ0Hc0WnU');

    this.employeeFBDocWatcher = this.employeeFBDoc.valueChanges();

    this.employeeFBDocWatcher.subscribe((dataDoc) => {
      // console.log('on load', dataDoc);
      if (
        dataDoc.data &&
        JSON.stringify(this.data) !== JSON.stringify(dataDoc.data)
      ) {
        this.loading = false;

        this.data = this.SortEmployees(dataDoc.data);
        this.lastSavedData = JSON.parse(JSON.stringify(this.data));
        const user: User = JSON.parse(localStorage.getItem('user'));

        console.log(this.data);

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

  ngOnInit(): void {
    // console.log(this.data);
  }

  addCard() {
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

  getClass(data) {
    let classes = 'e-card-content ';

    if (data.Flag) {
      classes += 'Flag Flag' + data.Flag;
    }

    return classes;
  }

  saveBoard() {
    console.log('saved');

    // let counter = {};

    this.data.forEach((d) => {
      d.Promote ??= 0;
      d.Flag ??= 0;
      d.Role ??= 'NA';
      d.Level ??= 'NA';

      if (isNaN(d.RankId)) {
        d.RankId = 0;
      }

      // if (!counter[d.Area]) {
      //   counter[d.Area] = 0;
      // }

      // if (!d.RankId || +d.RankId < 2) {
      //   d.RankId = counter[d.Area]++;
      // }
      // delete d.RankId;
      // delete d.RankID;

      // console.log(
      //   d.Name + ': ' + d.RankId + ' ' + d.Area + ': ' + counter[d.Area]
      // );
    });

    // console.log('on complete: ', this.data);

    // this.employeeFBDocWatcher.unsubscribe();
    this.employeeFBDoc.set(
      {
        data: this.data,
        backup: null,
      },
      { merge: true }
    );
    this.lastSavedData = JSON.parse(JSON.stringify(this.data));
    // this.sub();

    this.reset();
  }

  getTotal(text) {
    const area = this.data.filter((f) => f.Area === text);

    return (
      area.length +
      '/' +
      this.data.length +
      ' (' +
      ((100 * area.length) / this.data.length).toFixed(1) +
      '%)'
    );
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

    //  console.log(this.data);

    // if (JSON.stringify(this.kb.dataSource) !== JSON.stringify(this.lastSavedData )) {
    this.dataChanged = true;
    this.actionCount++;
    // }

    // console.log(this.dataChanged);
    // console.log(JSON.stringify(this.kb.dataSource));
    // console.log(JSON.stringify(this.lastSavedData));
    // this.saveBoard();

    // this.kb.dataSource = employees;
    // console.log({ employees });
  }

  private SortEmployees(source) {
    const employeeAreas = {};
    source.forEach((employee) => {
      if (!employeeAreas[employee.Area]) {
        employeeAreas[employee.Area] = [];
      }
      employeeAreas[employee.Area].push(employee);
    });
    // there might be a simpler way for this
    const keys = ['Always', 'Often', 'Meets', 'Below'];

    const employees = [];
    let OverAllRankCounter = 1;
    keys.forEach((k) => {
      if (employeeAreas[k] && employeeAreas[k].length > 0) {
        employeeAreas[k] = employeeAreas[k].filter(Boolean);

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

  reset() {
    this.kb.dataSource = this.lastSavedData;
    try {
      this.spreadsheet ??= {};
      this.spreadsheet['dataSource'] = this.lastSavedData;
    } catch (err) {
      console.error(err);
    }
    this.data = JSON.parse(JSON.stringify(this.lastSavedData));
    this.actionCount = 0;
    this.dataChanged = true;
  }

  beforeSave(args) {
    args.isFullPost = false;
    args.needBlobData = true;

    console.log(args);

    console.log(this.spreadsheet);
  }

  saveComplete(args) {
    console.log(args);

    alert("haven't implemented save yet - copy paste to excel");
  }

  dataBound(args: any) {
    // console.log({ args });
    // console.log( this.spreadsheet   );
    // console.log( this.spreadsheet.sheets    );
    // for (const col of this.spreadsheet.sheets[0].columns  ) {
    //   switch (col.field) {
    //     case 'Id':
    //       col.width = 50;
    //       col.index = 0;
    //       break;
    //     case 'OverAllRank':
    //       col.width = 50;
    //       col.index = 1;
    //       break;
    //     case 'Name':
    //       col.width = 150;
    //       col.index = 2;
    //       break;
    //   }
    // }
    // this.spreadsheet.refreshColumns();
  }
}
