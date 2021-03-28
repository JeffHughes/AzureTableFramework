import { Component } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth';
import { AngularFirestore } from '@angular/fire/firestore';
import { User } from './login/login.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  loggedIn = false;
  user: any = {};
  checkLoggedInInterval;

  constructor(public afAuth: AngularFireAuth, public afs: AngularFirestore) {
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
  isLoggedIn(): boolean {
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
