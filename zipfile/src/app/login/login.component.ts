import { Component, OnInit } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth';
import {
  AngularFirestore,
  AngularFirestoreDocument,
} from '@angular/fire/firestore';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  userData: any;

  constructor(public afAuth: AngularFireAuth, public afs: AngularFirestore) {}

  ngOnInit(): void {
    this.afAuth.authState.subscribe((user) => {
      if (user) {
        this.userData = user;
        localStorage.setItem('user', JSON.stringify(this.userData));
        JSON.parse(localStorage.getItem('user'));
      } else {
        localStorage.setItem('user', null);
        JSON.parse(localStorage.getItem('user'));
      }
    });
  }

  message = '';

  showPassword = false;
  usernameEntered(): void {
    this.showPassword = true;
  }

  // Sign in with email/password
  SignIn(email, password): any {
    const e = email.value + '@tradestation.com';
    const p = password.value;

    return this.afAuth
      .signInWithEmailAndPassword(e, p)
      .then(async (result) => {
        if (result.user.emailVerified) {
          this.SetUserData(result.user);
        } else {
          const verification = await this.sendEmailVerification();
          this.message = 'verify your email';
        }
      })
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

  showResetPassword = false;
  resetPassword(userName) {
    const e = userName.value + '@tradestation.com';
    this.ForgotPassword(e);
    this.showResetPassword = false;
    this.message = 'check your email';
  }

  async sendEmailVerification() {
    const unverifiedUser = await this.afAuth.currentUser;
    const response = await unverifiedUser.sendEmailVerification();
  }

  SignUp(email, password): any {
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

  SendVerificationMail(): any {
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
    const userRef: AngularFirestoreDocument<any> = this.afs.doc(
      `users/${user.uid}`
    );
    const userData: User = {
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

export interface User {
  uid: string;
  email: string;
  displayName: string;
  photoURL: string;
  emailVerified: boolean;
}
