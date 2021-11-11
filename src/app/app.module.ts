import { NgModule } from '@angular/core';
import { AngularFireModule } from '@angular/fire';
import { BrowserModule } from '@angular/platform-browser';
import { ButtonModule } from '@syncfusion/ej2-angular-buttons';
import { KanbanModule } from '@syncfusion/ej2-angular-kanban';

import { AppComponent } from './app.component';
import { KanbanComponent } from './kanban/kanban.component';
import { AngularFirestoreModule } from '@angular/fire/firestore';
import { AngularFireStorageModule } from '@angular/fire/storage';
import { AngularFireAuthModule } from '@angular/fire/auth';
import { LoginComponent } from './login/login.component';
import { SpreadsheetAllModule } from '@syncfusion/ej2-angular-spreadsheet';
import { ChartComponent } from './chart/chart.component';
import {   HighchartsChartModule } from 'highcharts-angular';

const firebaseConfig = {
  apiKey: 'AIzaSyDcxvR57JigS-jaP8ssmE0hnp2hyHFKNAQ',
  authDomain: 'tsemployeerank.firebaseapp.com',
  projectId: 'tsemployeerank',
  storageBucket: 'tsemployeerank.appspot.com',
  messagingSenderId: '35049950693',
  appId: '1:35049950693:web:370d5bd98bac69fb06730c'
};

@NgModule({
  declarations: [AppComponent, KanbanComponent, LoginComponent, ChartComponent],
  imports: [
    BrowserModule,
    KanbanModule,
    ButtonModule,
    SpreadsheetAllModule,
    AngularFireModule.initializeApp(firebaseConfig),
    AngularFirestoreModule, // firestore
    AngularFireAuthModule, // auth
    AngularFireStorageModule, // storage
    HighchartsChartModule
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
