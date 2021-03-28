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
      // { key: 'Promote', type: 'DropDown' },
      // { key: 'Flag', type: 'DropDown' },
    ],
  };

  employeeFBDoc;
  employeeFBDocWatcher;

  viewable = false;
  isAdmin = false;
  isUser = false;

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
        this.data = dataDoc.data;
        this.lastSavedData = JSON.parse(JSON.stringify(this.data));
        const user: User = JSON.parse(localStorage.getItem('user'));

        const username = user.email
          .toLowerCase()
          .replace('@tradestation.com', '');

        this.isAdmin = false;
        this.viewable = false;

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
        backup: this.data,
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
      this.SortEmployees();
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
  }

  private SortEmployees() {
    const employeeAreas = {};
    this.kb.dataSource.forEach((employee) => {
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
      employeeAreas[k].sort((a, b) => (a.RankId > b.RankId ? 1 : -1));
      let counter = 1;
      employeeAreas[k].forEach((e) => {
        e.RankId = counter++;
        e.OverAllRank = OverAllRankCounter++;
        employees.push(e);
      });
    });

    this.kb.dataSource = employees;
  }

  reset() {
    this.kb.dataSource = this.lastSavedData;
    this.data = JSON.parse(JSON.stringify(this.lastSavedData));
    this.actionCount = 0;
    this.dataChanged = true;
  }
}
