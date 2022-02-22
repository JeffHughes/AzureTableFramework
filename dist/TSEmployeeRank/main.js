(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["main"],{

/***/ 0:
/*!***************************!*\
  !*** multi ./src/main.ts ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! E:\tempdev\Prev\TSEmployeeRank\src\main.ts */"zUnb");


/***/ }),

/***/ "AytR":
/*!*****************************************!*\
  !*** ./src/environments/environment.ts ***!
  \*****************************************/
/*! exports provided: environment */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "environment", function() { return environment; });
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
const environment = {
    production: false
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.


/***/ }),

/***/ "Bho8":
/*!******************************************!*\
  !*** ./src/app/chart/chart.component.ts ***!
  \******************************************/
/*! exports provided: ChartComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ChartComponent", function() { return ChartComponent; });
/* harmony import */ var highcharts__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! highcharts */ "6n/F");
/* harmony import */ var highcharts__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(highcharts__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var highcharts_angular__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! highcharts-angular */ "kAVD");



class ChartComponent {
    constructor() {
        this.highcharts = highcharts__WEBPACK_IMPORTED_MODULE_0__;
        this.chartOptions = {
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
            },
            xAxis: {
                categories: ['Always', 'Often', 'Meets', 'Below'],
                visible: false,
            },
            yAxis: {
                visible: false,
            },
            tooltip: {
                pointFormat: '{point.label} has <b>{point.y:,.0f}% </b> of employees',
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
                    data: [17.5, 37.5, 45.0, 0],
                },
            ],
            credits: {
                enabled: false,
            },
            legend: { enabled: false },
        };
    }
    ngOnInit() { }
}
ChartComponent.ɵfac = function ChartComponent_Factory(t) { return new (t || ChartComponent)(); };
ChartComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: ChartComponent, selectors: [["app-chart"]], decls: 1, vars: 2, consts: [[2, "width", "100%", "height", "400px", "display", "block", "position", "absolute", "top", "50px", "z-index", "5", "background-color", "transparent", 3, "Highcharts", "options"]], template: function ChartComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](0, "highcharts-chart", 0);
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("Highcharts", ctx.highcharts)("options", ctx.chartOptions);
    } }, directives: [highcharts_angular__WEBPACK_IMPORTED_MODULE_2__["HighchartsChartComponent"]], styles: ["\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJjaGFydC5jb21wb25lbnQuc2NzcyJ9 */"] });


/***/ }),

/***/ "Sy1n":
/*!**********************************!*\
  !*** ./src/app/app.component.ts ***!
  \**********************************/
/*! exports provided: AppComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppComponent", function() { return AppComponent; });
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_fire_auth__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/fire/auth */ "UbJi");
/* harmony import */ var _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/fire/firestore */ "I/3d");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _kanban_kanban_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./kanban/kanban.component */ "d5Qy");
/* harmony import */ var _login_login_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./login/login.component */ "vtpD");






function AppComponent_div_0_Template(rf, ctx) { if (rf & 1) {
    const _r4 = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](0, "div", 3);
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](1, "a", 4);
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵlistener"]("click", function AppComponent_div_0_Template_a_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵrestoreView"](_r4); const ctx_r3 = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵnextContext"](); return ctx_r3.SignOut(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r0 = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtextInterpolate1"](" Logout ", ctx_r0.user.displayName || ctx_r0.user.email.replace("@tradestation.com", ""), " ");
} }
function AppComponent_app_kanban_1_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelement"](0, "app-kanban");
} }
function AppComponent_div_2_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementStart"](0, "div", 5);
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelement"](1, "app-login");
    _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵelementEnd"]();
} }
class AppComponent {
    constructor(afAuth, afs) {
        this.afAuth = afAuth;
        this.afs = afs;
        this.loggedIn = false;
        this.user = {};
        if (this.isLoggedIn()) {
            this.loggedIn = true;
            // console.log('logged in', this.user);
            clearInterval(this.checkLoggedInInterval);
        }
        this.checkLoggedInInterval = setInterval(() => {
            if (this.isLoggedIn()) {
                this.loggedIn = true;
                // console.log('logged in', this.user);
                clearInterval(this.checkLoggedInInterval);
            }
        }, 500);
    }
    // Returns true when user is looged in and email is verified
    isLoggedIn() {
        this.user = JSON.parse(localStorage.getItem('user'));
        return this.user !== null && this.user.emailVerified !== false
            ? true
            : false;
    }
    // Sign out
    SignOut() {
        return this.afAuth.signOut().then(() => {
            localStorage.removeItem('user');
        });
    }
}
AppComponent.ɵfac = function AppComponent_Factory(t) { return new (t || AppComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdirectiveInject"](_angular_fire_auth__WEBPACK_IMPORTED_MODULE_1__["AngularFireAuth"]), _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdirectiveInject"](_angular_fire_firestore__WEBPACK_IMPORTED_MODULE_2__["AngularFirestore"])); };
AppComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵdefineComponent"]({ type: AppComponent, selectors: [["app-root"]], decls: 3, vars: 3, consts: [["style", "\n    position: fixed;\n    top: 0;\n    right: 0;\n    z-index: 1;\n    width: 100%;\n    text-align: right;\n    font-size: small;\n    background-color: #006DB8;\n\n    background-size: auto 100% !important;\n    color: white;\n    padding: 5px;\n  ", 4, "ngIf"], [4, "ngIf"], ["class", "center-center", 4, "ngIf"], [2, "position", "fixed", "top", "0", "right", "0", "z-index", "1", "width", "100%", "text-align", "right", "font-size", "small", "background-color", "#006DB8", "background-size", "auto 100% !important", "color", "white", "padding", "5px"], ["href", "", 2, "color", "white", "text-decoration", "none", 3, "click"], [1, "center-center"]], template: function AppComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtemplate"](0, AppComponent_div_0_Template, 3, 1, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtemplate"](1, AppComponent_app_kanban_1_Template, 1, 0, "app-kanban", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵtemplate"](2, AppComponent_div_2_Template, 2, 0, "div", 2);
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵproperty"]("ngIf", ctx.loggedIn);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵproperty"]("ngIf", ctx.loggedIn);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_0__["ɵɵproperty"]("ngIf", !ctx.loggedIn);
    } }, directives: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["NgIf"], _kanban_kanban_component__WEBPACK_IMPORTED_MODULE_4__["KanbanComponent"], _login_login_component__WEBPACK_IMPORTED_MODULE_5__["LoginComponent"]], styles: ["\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IiIsImZpbGUiOiJhcHAuY29tcG9uZW50LnNjc3MifQ== */"] });


/***/ }),

/***/ "ZAI4":
/*!*******************************!*\
  !*** ./src/app/app.module.ts ***!
  \*******************************/
/*! exports provided: AppModule */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "AppModule", function() { return AppModule; });
/* harmony import */ var _angular_fire__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/fire */ "spgP");
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/platform-browser */ "jhN1");
/* harmony import */ var _syncfusion_ej2_angular_buttons__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @syncfusion/ej2-angular-buttons */ "SRvG");
/* harmony import */ var _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @syncfusion/ej2-angular-kanban */ "+0m8");
/* harmony import */ var _app_component__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./app.component */ "Sy1n");
/* harmony import */ var _kanban_kanban_component__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./kanban/kanban.component */ "d5Qy");
/* harmony import */ var _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @angular/fire/firestore */ "I/3d");
/* harmony import */ var _angular_fire_storage__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! @angular/fire/storage */ "Vaw3");
/* harmony import */ var _angular_fire_auth__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! @angular/fire/auth */ "UbJi");
/* harmony import */ var _login_login_component__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./login/login.component */ "vtpD");
/* harmony import */ var _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_10__ = __webpack_require__(/*! @syncfusion/ej2-angular-spreadsheet */ "940x");
/* harmony import */ var _chart_chart_component__WEBPACK_IMPORTED_MODULE_11__ = __webpack_require__(/*! ./chart/chart.component */ "Bho8");
/* harmony import */ var highcharts_angular__WEBPACK_IMPORTED_MODULE_12__ = __webpack_require__(/*! highcharts-angular */ "kAVD");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_13__ = __webpack_require__(/*! @angular/core */ "fXoL");















const firebaseConfig = {
    apiKey: 'AIzaSyDcxvR57JigS-jaP8ssmE0hnp2hyHFKNAQ',
    authDomain: 'tsemployeerank.firebaseapp.com',
    projectId: 'tsemployeerank',
    storageBucket: 'tsemployeerank.appspot.com',
    messagingSenderId: '35049950693',
    appId: '1:35049950693:web:370d5bd98bac69fb06730c'
};
class AppModule {
}
AppModule.ɵfac = function AppModule_Factory(t) { return new (t || AppModule)(); };
AppModule.ɵmod = _angular_core__WEBPACK_IMPORTED_MODULE_13__["ɵɵdefineNgModule"]({ type: AppModule, bootstrap: [_app_component__WEBPACK_IMPORTED_MODULE_4__["AppComponent"]] });
AppModule.ɵinj = _angular_core__WEBPACK_IMPORTED_MODULE_13__["ɵɵdefineInjector"]({ providers: [], imports: [[
            _angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["BrowserModule"],
            _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_3__["KanbanModule"],
            _syncfusion_ej2_angular_buttons__WEBPACK_IMPORTED_MODULE_2__["ButtonModule"],
            _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_10__["SpreadsheetAllModule"],
            _angular_fire__WEBPACK_IMPORTED_MODULE_0__["AngularFireModule"].initializeApp(firebaseConfig),
            _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_6__["AngularFirestoreModule"],
            _angular_fire_auth__WEBPACK_IMPORTED_MODULE_8__["AngularFireAuthModule"],
            _angular_fire_storage__WEBPACK_IMPORTED_MODULE_7__["AngularFireStorageModule"],
            highcharts_angular__WEBPACK_IMPORTED_MODULE_12__["HighchartsChartModule"]
        ]] });
(function () { (typeof ngJitMode === "undefined" || ngJitMode) && _angular_core__WEBPACK_IMPORTED_MODULE_13__["ɵɵsetNgModuleScope"](AppModule, { declarations: [_app_component__WEBPACK_IMPORTED_MODULE_4__["AppComponent"], _kanban_kanban_component__WEBPACK_IMPORTED_MODULE_5__["KanbanComponent"], _login_login_component__WEBPACK_IMPORTED_MODULE_9__["LoginComponent"], _chart_chart_component__WEBPACK_IMPORTED_MODULE_11__["ChartComponent"]], imports: [_angular_platform_browser__WEBPACK_IMPORTED_MODULE_1__["BrowserModule"],
        _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_3__["KanbanModule"],
        _syncfusion_ej2_angular_buttons__WEBPACK_IMPORTED_MODULE_2__["ButtonModule"],
        _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_10__["SpreadsheetAllModule"], _angular_fire__WEBPACK_IMPORTED_MODULE_0__["AngularFireModule"], _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_6__["AngularFirestoreModule"],
        _angular_fire_auth__WEBPACK_IMPORTED_MODULE_8__["AngularFireAuthModule"],
        _angular_fire_storage__WEBPACK_IMPORTED_MODULE_7__["AngularFireStorageModule"],
        highcharts_angular__WEBPACK_IMPORTED_MODULE_12__["HighchartsChartModule"]] }); })();


/***/ }),

/***/ "d5Qy":
/*!********************************************!*\
  !*** ./src/app/kanban/kanban.component.ts ***!
  \********************************************/
/*! exports provided: KanbanComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "KanbanComponent", function() { return KanbanComponent; });
/* harmony import */ var highcharts__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! highcharts */ "6n/F");
/* harmony import */ var highcharts__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(highcharts__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/fire/firestore */ "I/3d");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @syncfusion/ej2-angular-kanban */ "+0m8");
/* harmony import */ var _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @syncfusion/ej2-angular-spreadsheet */ "940x");
/* harmony import */ var highcharts_angular__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! highcharts-angular */ "kAVD");







const _c0 = ["kb"];
const _c1 = ["spreadsheet"];
function KanbanComponent_ng_container_0_div_1_ng_container_5_ng_container_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, "s");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
} }
function KanbanComponent_ng_container_0_div_1_ng_container_5_Template(rf, ctx) { if (rf & 1) {
    const _r22 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "a", 17);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_div_1_ng_container_5_Template_a_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r22); const ctx_r21 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3); return ctx_r21.saveBoard(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](2, "img", 19);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](4, KanbanComponent_ng_container_0_div_1_ng_container_5_ng_container_4_Template, 2, 0, "ng-container", 0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](5, " \u00A0 \u00A0 ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](6, "a", 17);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_div_1_ng_container_5_Template_a_click_6_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r22); const ctx_r23 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3); return ctx_r23.reset(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](7, "img", 20);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](8, " reset to last publish ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
} if (rf & 2) {
    const ctx_r19 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" publish ", ctx_r19.actionCount, " action");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r19.actionCount != 1);
} }
function KanbanComponent_ng_container_0_div_1_Template(rf, ctx) { if (rf & 1) {
    const _r25 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 16);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "a", 17);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_div_1_Template_a_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r25); const ctx_r24 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r24.addCard(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](2, "img", 18);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3, " add employee ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](4, " \u00A0 \u00A0 ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](5, KanbanComponent_ng_container_0_div_1_ng_container_5_Template, 9, 2, "ng-container", 0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r1 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](5);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r1.actionCount && ctx_r1.actionCount > 0);
} }
function KanbanComponent_ng_container_0_highcharts_chart_3_Template(rf, ctx) { if (rf & 1) {
    const _r27 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "highcharts-chart", 21);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_highcharts_chart_3_Template_highcharts_chart_click_0_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r27); const ctx_r26 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r26.showCurve = false; })("updateChange", function KanbanComponent_ng_container_0_highcharts_chart_3_Template_highcharts_chart_updateChange_0_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r27); const ctx_r28 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r28.updateFlag = $event; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r2 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("Highcharts", ctx_r2.highcharts)("options", ctx_r2.chartOptions)("update", ctx_r2.updateFlag);
} }
function KanbanComponent_ng_container_0_ng_template_8_div_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r30 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r30.getIdeal(0), " ");
} }
function KanbanComponent_ng_container_0_ng_template_8_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](4, KanbanComponent_ng_container_0_ng_template_8_div_4_Template, 2, 1, "div", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r29 = ctx.$implicit;
    const ctx_r5 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r5.getTotal(data_r29.headerText), " ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r5.showCurve);
} }
function KanbanComponent_ng_container_0_ng_template_11_div_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r32 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r32.getIdeal(1), " ");
} }
function KanbanComponent_ng_container_0_ng_template_11_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](4, KanbanComponent_ng_container_0_ng_template_11_div_4_Template, 2, 1, "div", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r31 = ctx.$implicit;
    const ctx_r7 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r7.getTotal(data_r31.headerText), " ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r7.showCurve);
} }
function KanbanComponent_ng_container_0_ng_template_14_div_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r34 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r34.getIdeal(2), " ");
} }
function KanbanComponent_ng_container_0_ng_template_14_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](4, KanbanComponent_ng_container_0_ng_template_14_div_4_Template, 2, 1, "div", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r33 = ctx.$implicit;
    const ctx_r9 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r9.getTotal(data_r33.headerText), " ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r9.showCurve);
} }
function KanbanComponent_ng_container_0_ng_template_17_div_4_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 26);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r36 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r36.getIdeal(3), " ");
} }
function KanbanComponent_ng_container_0_ng_template_17_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 22);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 23);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 24);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](4, KanbanComponent_ng_container_0_ng_template_17_div_4_Template, 2, 1, "div", 25);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r35 = ctx.$implicit;
    const ctx_r11 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx_r11.getTotal(data_r35.headerText), " ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r11.showCurve);
} }
function KanbanComponent_ng_container_0_ng_template_19_ng_container_5_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](1, "img", 28);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
} }
function KanbanComponent_ng_container_0_ng_template_19_div_6_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 29);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r37 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]().$implicit;
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", data_r37.Notes, " ");
} }
function KanbanComponent_ng_container_0_ng_template_19_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](4, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](5, KanbanComponent_ng_container_0_ng_template_19_ng_container_5_Template, 2, 0, "ng-container", 0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](6, KanbanComponent_ng_container_0_ng_template_19_div_6_Template, 2, 1, "div", 27);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const data_r37 = ctx.$implicit;
    const ctx_r13 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵclassMap"](ctx_r13.getClass(data_r37));
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate4"](" ", data_r37.RankId, ". ", data_r37.Name, " ", data_r37.Role, "-", data_r37.Level, " ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", data_r37.Promote);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", data_r37.Notes);
} }
function KanbanComponent_ng_container_0_div_22_Template(rf, ctx) { if (rf & 1) {
    const _r42 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 30);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "a", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_div_22_Template_a_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r42); const ctx_r41 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r41.showSpreadsheet = true; });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "show spreadsheet");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function KanbanComponent_ng_container_0_div_23_Template(rf, ctx) { if (rf & 1) {
    const _r44 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "a", 31);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function KanbanComponent_ng_container_0_div_23_Template_a_click_1_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r44); const ctx_r43 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r43.updateChartData(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "show curve");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function KanbanComponent_ng_container_0_div_24_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 32);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, " click anywhere to hide curve ");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
const _c2 = function () { return { selectCells: true, formatCells: false, formatRows: false, formatColumns: false, insertLink: false }; };
function KanbanComponent_ng_container_0_div_25_Template(rf, ctx) { if (rf & 1) {
    const _r47 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 33);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "ejs-spreadsheet", 34, 35);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("dataBound", function KanbanComponent_ng_container_0_div_25_Template_ejs_spreadsheet_dataBound_1_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r47); const ctx_r46 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2); return ctx_r46.dataBound($event); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](3, "e-sheets");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](4, "e-sheet", 36);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](5, "e-ranges");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](6, "e-range", 37);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](7, "e-columns");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](8, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](9, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](10, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](11, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](12, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](13, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](14, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](15, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](16, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](17, "e-column", 38);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} if (rf & 2) {
    const ctx_r17 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("showFile", false);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("allowSave", true)("protectSettings", _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵpureFunction0"](14, _c2));
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("dataSource", ctx_r17.data);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 90);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 170);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 78);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 56);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 40);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 36);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 60);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 40);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 150);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("width", 1);
} }
function KanbanComponent_ng_container_0_ng_container_28_Template(rf, ctx) { if (rf & 1) {
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "div");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](2, "sorry, you don't have permissions to view this.");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
} }
function KanbanComponent_ng_container_0_Template(rf, ctx) { if (rf & 1) {
    const _r49 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerStart"](0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](1, KanbanComponent_ng_container_0_div_1_Template, 6, 1, "div", 1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](3, KanbanComponent_ng_container_0_highcharts_chart_3_Template, 1, 3, "highcharts-chart", 3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](4, "ejs-kanban", 4, 5);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("actionComplete", function KanbanComponent_ng_container_0_Template_ejs_kanban_actionComplete_4_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r49); const ctx_r48 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); return ctx_r48.actionComplete(); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](6, "e-columns");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](7, "e-column", 6);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](8, KanbanComponent_ng_container_0_ng_template_8_Template, 5, 2, "ng-template", null, 7, _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplateRefExtractor"]);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](10, "e-column", 8);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](11, KanbanComponent_ng_container_0_ng_template_11_Template, 5, 2, "ng-template", null, 7, _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplateRefExtractor"]);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](13, "e-column", 9);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](14, KanbanComponent_ng_container_0_ng_template_14_Template, 5, 2, "ng-template", null, 7, _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplateRefExtractor"]);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](16, "e-column", 10);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](17, KanbanComponent_ng_container_0_ng_template_17_Template, 5, 2, "ng-template", null, 7, _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplateRefExtractor"]);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](19, KanbanComponent_ng_container_0_ng_template_19_Template, 7, 8, "ng-template", null, 11, _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplateRefExtractor"]);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](21, "div", 12);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](22, KanbanComponent_ng_container_0_div_22_Template, 3, 0, "div", 13);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](23, KanbanComponent_ng_container_0_div_23_Template, 3, 0, "div", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](24, KanbanComponent_ng_container_0_div_24_Template, 2, 0, "div", 14);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](25, KanbanComponent_ng_container_0_div_25_Template, 18, 15, "div", 15);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](26, "br");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](27, "br");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](28, KanbanComponent_ng_container_0_ng_container_28_Template, 3, 0, "ng-container", 0);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementContainerEnd"]();
} if (rf & 2) {
    const ctx_r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r0.isAdmin);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](2);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r0.showCurve);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("dataSource", ctx_r0.data)("cardSettings", ctx_r0.cardSettings)("dialogSettings", ctx_r0.dialogSettings)("sortSettings", ctx_r0.sortSettings);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](18);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", !ctx_r0.showSpreadsheet);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", !ctx_r0.showCurve);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r0.showCurve);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx_r0.showSpreadsheet);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](3);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", !ctx_r0.viewable && !ctx_r0.loading);
} }
class KanbanComponent {
    constructor(db) {
        this.db = db;
        this.data = [];
        this.showCurve = false;
        this.updateFlag = false;
        this.highcharts = highcharts__WEBPACK_IMPORTED_MODULE_0__;
        this.chartOptions = {
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
                pointFormat: '{series.name} {point.label} has <b>{point.y:,.1f}% </b> of employees',
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
        this.sortSettings = {
            field: 'RankId',
        };
        this.cardSettings = {
            contentField: 'Notes',
            headerField: 'Id',
        };
        this.dialogSettings = {
            fields: [
                { key: 'Name', type: 'TextBox' },
                { key: 'Area', type: 'DropDown' },
                { key: 'Role', type: 'TextBox' },
                { key: 'Level', type: 'TextBox' },
                { key: 'Notes', type: 'TextArea' },
                { key: 'Promote', type: 'Numeric' },
                { key: 'Flag', type: 'Numeric' },
                { key: 'HiPer', type: 'Numeric' },
                { key: 'HiPo', type: 'Numeric' },
            ],
        };
        this.viewable = false;
        this.isAdmin = false;
        this.isUser = false;
        this.showSpreadsheet = false;
        this.loading = true;
        this.actionCount = 0;
        this.dataChanged = true;
        this.keys = ['Always', 'Often', 'Meets', 'Below'];
        this.updateChartDataTimeout = null;
        this.sub();
    }
    sub() {
        this.employeeFBDoc = this.db
            .collection('employees')
            .doc('G4ESm6jZpjhkJ0Hc0WnU');
        this.employeeFBDocWatcher = this.employeeFBDoc.valueChanges();
        this.employeeFBDocWatcher.subscribe((dataDoc) => {
            if (dataDoc.data &&
                JSON.stringify(this.data) !== JSON.stringify(dataDoc.data)) {
                this.loading = false;
                this.data = this.SortEmployees(dataDoc.data);
                this.lastSavedData = JSON.parse(JSON.stringify(this.data));
                const user = JSON.parse(localStorage.getItem('user'));
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
    addCard() {
        const id = this.data.length + 1;
        const card = {
            Id: id,
            Area: 'Meets',
            Notes: '',
            Promote: 0,
            Flag: 0,
            HiPer: 0,
            HiPo: 0,
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
        this.data.forEach((d) => {
            var _a, _b, _c, _d, _e, _f;
            (_a = d.Promote) !== null && _a !== void 0 ? _a : (d.Promote = 0);
            (_b = d.Flag) !== null && _b !== void 0 ? _b : (d.Flag = 0);
            (_c = d.Role) !== null && _c !== void 0 ? _c : (d.Role = 'NA');
            (_d = d.Level) !== null && _d !== void 0 ? _d : (d.Level = 'NA');
            (_e = d.HiPer) !== null && _e !== void 0 ? _e : (d.HiPer = 0);
            (_f = d.HiPo) !== null && _f !== void 0 ? _f : (d.HiPo = 0);
            if (isNaN(d.RankId)) {
                d.RankId = this.data.length + 1;
            }
        });
        this.employeeFBDoc.set({
            data: this.data,
            backup: null,
        }, { merge: true });
        this.lastSavedData = JSON.parse(JSON.stringify(this.data));
        this.reset();
    }
    getTotal(text) {
        const area = this.data.filter((f) => f.Area === text);
        return (text +
            ' ' +
            area.length +
            // '/' +
            // this.data.length +
            ' (' +
            ((100 * area.length) / this.data.length).toFixed(1) +
            '%)');
    }
    getIdeal(num) {
        const percent = this.chartOptions.series[0].data[num].toFixed(1);
        const roundNum = Math.floor(+percent * this.data.length / 100);
        return roundNum + ' (' + percent + '%)';
    }
    actionComplete() {
        setTimeout(() => {
            var _a;
            this.data = this.SortEmployees(this.kb.dataSource);
            this.kb.dataSource = this.data;
            try {
                (_a = this.spreadsheet) !== null && _a !== void 0 ? _a : (this.spreadsheet = {});
                this.spreadsheet.dataSource = this.data;
            }
            catch (err) {
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
    SortEmployees(source) {
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
    updateChartData() {
        if (this.data.length > 4) {
            clearTimeout(this.updateChartDataTimeout);
            this.updateChartDataTimeout = setTimeout(() => {
                const seriesData = [];
                this.keys.forEach((k) => {
                    const areaEmployees = this.data.filter((f) => f.Area === k);
                    const percentageOfTotal = (100 * areaEmployees.length) / this.data.length;
                    if (percentageOfTotal >= 0 && percentageOfTotal <= 100) {
                        seriesData.push(percentageOfTotal);
                    }
                    else {
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
    sortKeys(item) {
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
            HiPer: item.HiPer,
            HiPo: item.HiPo
        };
    }
    reset() {
        var _a;
        this.kb.dataSource = this.lastSavedData;
        try {
            (_a = this.spreadsheet) !== null && _a !== void 0 ? _a : (this.spreadsheet = {});
            this.spreadsheet.dataSource = this.lastSavedData;
        }
        catch (err) {
            console.error(err);
        }
        this.data = JSON.parse(JSON.stringify(this.lastSavedData));
        this.actionCount = 0;
        this.dataChanged = true;
        this.showCurve = false;
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
}
KanbanComponent.ɵfac = function KanbanComponent_Factory(t) { return new (t || KanbanComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_fire_firestore__WEBPACK_IMPORTED_MODULE_2__["AngularFirestore"])); };
KanbanComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: KanbanComponent, selectors: [["app-kanban"]], viewQuery: function KanbanComponent_Query(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵviewQuery"](_c0, 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵviewQuery"](_c1, 1);
    } if (rf & 2) {
        let _t;
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx.kb = _t.first);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵqueryRefresh"](_t = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵloadQuery"]()) && (ctx.spreadsheet = _t.first);
    } }, decls: 1, vars: 1, consts: [[4, "ngIf"], ["style", "\n      position: fixed;\n      top: -2.5px;\n      left: 0;\n      z-index: 30;\n\n      text-align: left;\n\n      color: white;\n      padding: 5px;\n    ", 4, "ngIf"], [2, "position", "relative"], ["style", "\n        width: 100%;\n        height: 100%;\n        position: absolute;\n        top: 50px;\n        z-index: 5;\n        background-color: transparent;\n        user-select: none;\n      ", 3, "Highcharts", "options", "update", "click", "updateChange", 4, "ngIf"], ["keyField", "Area", 2, "margin-top", "30px", 3, "dataSource", "cardSettings", "dialogSettings", "sortSettings", "actionComplete"], ["kb", ""], ["headerText", "Always", "keyField", "Always"], ["template", ""], ["headerText", "Often", "keyField", "Often"], ["headerText", "Meets", "keyField", "Meets"], ["headerText", "Below", "keyField", "Below"], ["cardSettingsTemplate", ""], [2, "display", "flex", "width", "100%", "justify-content", "space-between"], ["style", "margin-left: 10px", 4, "ngIf"], ["style", "margin-right: 10px", 4, "ngIf"], ["style", "height: 95vh", 4, "ngIf"], [2, "position", "fixed", "top", "-2.5px", "left", "0", "z-index", "30", "text-align", "left", "color", "white", "padding", "5px"], ["href", "#", 2, "color", "white", "text-decoration", "none", 3, "click"], ["src", "../../assets/add-15x15.png", 2, "height", "15px"], ["src", "../../assets/save-15x15.png", 2, "height", "15px"], ["src", "../../assets/reset-15x15.png", 2, "height", "15px"], [2, "width", "100%", "height", "100%", "position", "absolute", "top", "50px", "z-index", "5", "background-color", "transparent", "user-select", "none", 3, "Highcharts", "options", "update", "click", "updateChange"], [2, "width", "100%", "display", "flex", "justify-content", "center"], [2, "display", "flex", "flex-direction", "column", "align-items", "flex-end"], [1, "header-text"], ["class", "header-text-ideal", 4, "ngIf"], [1, "header-text-ideal"], ["style", "background-color: whitesmoke; width: 100%", 4, "ngIf"], ["src", "../../assets/promote.png", "height", "16"], [2, "background-color", "whitesmoke", "width", "100%"], [2, "margin-left", "10px"], ["href", "#", 3, "click"], [2, "margin-right", "10px"], [2, "height", "95vh"], [3, "showFile", "dataBound"], ["spreadsheet", ""], [3, "allowSave", "protectSettings"], [3, "dataSource"], [3, "width"]], template: function KanbanComponent_Template(rf, ctx) { if (rf & 1) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](0, KanbanComponent_ng_container_0_Template, 29, 11, "ng-container", 0);
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.viewable);
    } }, directives: [_angular_common__WEBPACK_IMPORTED_MODULE_3__["NgIf"], _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_4__["KanbanComponent"], _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_4__["ColumnsDirective"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["ColumnsDirective"], _syncfusion_ej2_angular_kanban__WEBPACK_IMPORTED_MODULE_4__["ColumnDirective"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["ColumnDirective"], highcharts_angular__WEBPACK_IMPORTED_MODULE_6__["HighchartsChartComponent"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["SpreadsheetComponent"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["SheetsDirective"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["SheetDirective"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["RangesDirective"], _syncfusion_ej2_angular_spreadsheet__WEBPACK_IMPORTED_MODULE_5__["RangeDirective"]], styles: [".Flag[_ngcontent-%COMP%] {\n  background-color: #ffffda;\n}\n\n.Flag1[_ngcontent-%COMP%] {\n  background-color: yellow;\n}\n\n.Flag2[_ngcontent-%COMP%] {\n  background-color: #ff8800;\n  color: white !important;\n}\n\n.header-text-ideal[_ngcontent-%COMP%] {\n  color: #008dd3;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIi4uXFwuLlxcLi5cXGthbmJhbi5jb21wb25lbnQuc2NzcyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTtFQUNFLHlCQUFBO0FBQ0Y7O0FBRUE7RUFDRSx3QkFBQTtBQUNGOztBQUVBO0VBQ0UseUJBQUE7RUFDQSx1QkFBQTtBQUNGOztBQUVBO0VBQ0UsY0FBQTtBQUNGIiwiZmlsZSI6ImthbmJhbi5jb21wb25lbnQuc2NzcyIsInNvdXJjZXNDb250ZW50IjpbIi5GbGFnIHtcclxuICBiYWNrZ3JvdW5kLWNvbG9yOiByZ2IoMjU1LCAyNTUsIDIxOCk7XHJcbn1cclxuXHJcbi5GbGFnMSB7XHJcbiAgYmFja2dyb3VuZC1jb2xvcjogeWVsbG93O1xyXG59XHJcblxyXG4uRmxhZzIge1xyXG4gIGJhY2tncm91bmQtY29sb3I6IHJnYigyNTUsIDEzNiwgMCk7XHJcbiAgY29sb3I6IHdoaXRlICFpbXBvcnRhbnQ7XHJcbn1cclxuXHJcbi5oZWFkZXItdGV4dC1pZGVhbCB7XHJcbiAgY29sb3I6ICMwMDhkZDM7XHJcbn1cclxuIl19 */"] });


/***/ }),

/***/ "vtpD":
/*!******************************************!*\
  !*** ./src/app/login/login.component.ts ***!
  \******************************************/
/*! exports provided: LoginComponent */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "LoginComponent", function() { return LoginComponent; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tslib */ "mrSG");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _angular_fire_auth__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @angular/fire/auth */ "UbJi");
/* harmony import */ var _angular_fire_firestore__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @angular/fire/firestore */ "I/3d");
/* harmony import */ var _angular_common__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @angular/common */ "ofXK");
/* harmony import */ var _syncfusion_ej2_angular_buttons__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @syncfusion/ej2-angular-buttons */ "SRvG");






function LoginComponent_a_23_Template(rf, ctx) { if (rf & 1) {
    const _r5 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "a", 15);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function LoginComponent_a_23_Template_a_click_0_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r5); const ctx_r4 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](10); return ctx_r4.resetPassword(_r0); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](1, "Send Reset Email");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
function LoginComponent_tr_27_Template(rf, ctx) { if (rf & 1) {
    const _r7 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "tr");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "td");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "div", 8);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](3, "a", 16);
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function LoginComponent_tr_27_Template_a_click_3_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r7); const ctx_r6 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵnextContext"](); const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](10); return ctx_r6.resetPassword(_r0.value); });
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](4, "Reset Password");
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
} }
class LoginComponent {
    constructor(afAuth, afs) {
        this.afAuth = afAuth;
        this.afs = afs;
        this.message = '';
        this.showPassword = false;
        this.showResetPassword = false;
    }
    ngOnInit() {
        this.afAuth.authState.subscribe((user) => {
            if (user) {
                this.userData = user;
                localStorage.setItem('user', JSON.stringify(this.userData));
                JSON.parse(localStorage.getItem('user'));
            }
            else {
                localStorage.setItem('user', null);
                JSON.parse(localStorage.getItem('user'));
            }
        });
    }
    usernameEntered() {
        this.showPassword = true;
    }
    // Sign in with email/password
    SignIn(email, password) {
        const e = email.value + '@tradestation.com';
        const p = password.value;
        return this.afAuth
            .signInWithEmailAndPassword(e, p)
            .then((result) => Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function* () {
            if (result.user.emailVerified) {
                this.SetUserData(result.user);
            }
            else {
                const verification = yield this.sendEmailVerification();
                this.message = 'verify your email';
            }
        }))
            .catch((error) => {
            console.log(error.message);
            switch (error.code) {
                case 'auth/wrong-password':
                    this.message = 'wrong password';
                    this.showResetPassword = true;
                    break;
                case 'auth/user-not-found':
                    this.SignUp(e, p);
                    this.message = 'check your email to verify';
                    break;
            }
        });
    }
    resetPassword(userName) {
        const e = userName.value + '@tradestation.com';
        this.ForgotPassword(e);
        this.showResetPassword = false;
        this.message = 'check your email';
    }
    sendEmailVerification() {
        return Object(tslib__WEBPACK_IMPORTED_MODULE_0__["__awaiter"])(this, void 0, void 0, function* () {
            const unverifiedUser = yield this.afAuth.currentUser;
            const response = yield unverifiedUser.sendEmailVerification();
        });
    }
    SignUp(email, password) {
        return this.afAuth
            .createUserWithEmailAndPassword(email, password)
            .then((result) => {
            /* Call the SendVerificaitonMail() function when new user sign
            up and returns promise */
            this.SendVerificationMail();
            this.SetUserData(result.user);
        })
            .catch((error) => {
            window.alert(error.message);
        });
    }
    SendVerificationMail() {
        // return this.afAuth.currentUser.sendEmailVerification().then(() => {});
    }
    // Reset Forggot password
    ForgotPassword(passwordResetEmail) {
        return this.afAuth
            .sendPasswordResetEmail(passwordResetEmail)
            .then(() => {
            window.alert('Password reset email sent, check your inbox.');
        })
            .catch((error) => {
            window.alert(error);
        });
    }
    SetUserData(user) {
        const userRef = this.afs.doc(`users/${user.uid}`);
        const userData = {
            uid: user.uid,
            email: user.email,
            displayName: user.displayName,
            photoURL: user.photoURL,
            emailVerified: user.emailVerified,
        };
        return userRef.set(userData, {
            merge: true,
        });
    }
    keyDownFunction(event, userName, userPassword) {
        if (event.keyCode === 13) {
            this.SignIn(userName, userPassword);
        }
    }
}
LoginComponent.ɵfac = function LoginComponent_Factory(t) { return new (t || LoginComponent)(_angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_fire_auth__WEBPACK_IMPORTED_MODULE_2__["AngularFireAuth"]), _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdirectiveInject"](_angular_fire_firestore__WEBPACK_IMPORTED_MODULE_3__["AngularFirestore"])); };
LoginComponent.ɵcmp = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵdefineComponent"]({ type: LoginComponent, selectors: [["app-login"]], decls: 28, vars: 3, consts: [[1, "center-center", 3, "keydown"], [2, "border", "1px solid #0089cf"], [2, "font-size", "0"], ["src", "../../assets/TSLogo.png", 2, "width", "100%", "height", "100%"], [1, "std-pad", 2, "display", "flex"], ["type", "text", "placeholder", "Username", "required", "", 2, "text-align", "right"], ["userName", ""], [2, "text-align", "right"], [1, "std-pad"], ["type", "password", "placeholder", "Password", "required", "", 2, "width", "310px"], ["userPassword", ""], [1, "std-pad", 2, "width", "95%", "display", "flex", "justify-content", "space-between"], ["style", "text-decoration: underline;\n            cursor: pointer;\n            font-size: small;\n            ", 3, "click", 4, "ngIf"], ["ejs-button", "", 3, "click"], [4, "ngIf"], [2, "text-decoration", "underline", "cursor", "pointer", "font-size", "small", 3, "click"], [3, "click"]], template: function LoginComponent_Template(rf, ctx) { if (rf & 1) {
        const _r8 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵgetCurrentView"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](0, "div", 0);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("keydown", function LoginComponent_Template_div_keydown_0_listener($event) { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r8); const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](10); const _r1 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](17); return ctx.keyDownFunction($event, _r0, _r1); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](1, "table", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](2, "tr", 1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](3, "td", 2);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](4, "img", 3);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](5, "tr");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](6, "td");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](7, "div", 4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](8, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](9, "input", 5, 6);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](11, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](12, "@tradestation.com");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](13, "tr");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](14, "td", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](15, "div", 8);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelement"](16, "input", 9, 10);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](18, "tr");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](19, "td", 7);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](20, "div", 11);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](21, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](22);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](23, LoginComponent_a_23_Template, 2, 0, "a", 12);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](24, "div");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementStart"](25, "button", 13);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵlistener"]("click", function LoginComponent_Template_button_click_25_listener() { _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵrestoreView"](_r8); const _r0 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](10); const _r1 = _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵreference"](17); return ctx.SignIn(_r0, _r1); });
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtext"](26, " Next > ");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtemplate"](27, LoginComponent_tr_27_Template, 5, 0, "tr", 14);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵelementEnd"]();
    } if (rf & 2) {
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](22);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵtextInterpolate1"](" ", ctx.message, " ");
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](1);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.showResetPassword);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵadvance"](4);
        _angular_core__WEBPACK_IMPORTED_MODULE_1__["ɵɵproperty"]("ngIf", ctx.showPassword);
    } }, directives: [_angular_common__WEBPACK_IMPORTED_MODULE_4__["NgIf"], _syncfusion_ej2_angular_buttons__WEBPACK_IMPORTED_MODULE_5__["ButtonComponent"]], styles: ["table[_ngcontent-%COMP%] {\n  border-collapse: collapse;\n}\n\ntd[_ngcontent-%COMP%] {\n  padding: 0;\n  margin: 0;\n}\n\n.std-pad[_ngcontent-%COMP%] {\n  padding: 10px;\n}\n/*# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIi4uXFwuLlxcLi5cXGxvZ2luLmNvbXBvbmVudC5zY3NzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0VBQ0UseUJBQUE7QUFDRjs7QUFFQTtFQUNFLFVBQUE7RUFDQSxTQUFBO0FBQ0Y7O0FBRUE7RUFDRSxhQUFBO0FBQ0YiLCJmaWxlIjoibG9naW4uY29tcG9uZW50LnNjc3MiLCJzb3VyY2VzQ29udGVudCI6WyJ0YWJsZSB7XHJcbiAgYm9yZGVyLWNvbGxhcHNlOiBjb2xsYXBzZTtcclxufVxyXG5cclxudGQge1xyXG4gIHBhZGRpbmc6IDA7XHJcbiAgbWFyZ2luOiAwO1xyXG59XHJcblxyXG4uc3RkLXBhZCB7XHJcbiAgcGFkZGluZzogMTBweDtcclxufVxyXG4iXX0= */"] });


/***/ }),

/***/ "zUnb":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! @angular/platform-browser */ "jhN1");
/* harmony import */ var _angular_core__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @angular/core */ "fXoL");
/* harmony import */ var _app_app_module__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./app/app.module */ "ZAI4");
/* harmony import */ var _environments_environment__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./environments/environment */ "AytR");




if (_environments_environment__WEBPACK_IMPORTED_MODULE_3__["environment"].production) {
    Object(_angular_core__WEBPACK_IMPORTED_MODULE_1__["enableProdMode"])();
}
_angular_platform_browser__WEBPACK_IMPORTED_MODULE_0__["platformBrowser"]().bootstrapModule(_app_app_module__WEBPACK_IMPORTED_MODULE_2__["AppModule"])
    .catch(err => console.error(err));


/***/ }),

/***/ "zn8P":
/*!******************************************************!*\
  !*** ./$$_lazy_route_resource lazy namespace object ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncaught exception popping up in devtools
	return Promise.resolve().then(function() {
		var e = new Error("Cannot find module '" + req + "'");
		e.code = 'MODULE_NOT_FOUND';
		throw e;
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "zn8P";

/***/ })

},[[0,"runtime","vendor"]]]);
//# sourceMappingURL=main.js.map