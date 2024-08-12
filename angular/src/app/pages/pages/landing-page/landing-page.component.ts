import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from 'src/services/auth.service';
import { LoginDTO } from 'src/services/interfaces/auth/logindto.model';
import { AlertService } from 'src/services/alert.service';

@Component({
  selector: 'vex-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.scss'],
  animations: [
    fadeInUp400ms
  ]
})
export class LandingPageComponent implements OnInit {
  form: UntypedFormGroup;
  inputType = 'password';
  visible = false;
  
  isLogin = false;
  isRegister = false;
  constructor(
    private router: Router,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private snackbar: MatSnackBar,
    private authservice: AuthService,
    private alertService: AlertService
  ) { 
    this.clearLocalStorage();
  }

  ngOnInit() {
    this.clearLocalStorage();
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngAfterViewInit() {
    setTimeout(()=>{
      this.alertService.hideBlockUI();
    },1000); 
  }

  clearLocalStorage() {
    localStorage.setItem('CurrentUser', undefined);
    localStorage.setItem('CurrentUserRole', undefined);
    localStorage.setItem('Token', undefined);
  }

  send() {
    if (!this.form.valid) {
      this.snackbar.open('Login Failed', 'Please provide email and password.', { duration: 5000 });
      return;
    }

    let logindto = new LoginDTO();
    logindto.email = this.form.value.email;
    logindto.password = this.form.value.password;

    this.authservice.login(logindto).subscribe(result => {
      if (result.status === 200) {
        //TODO: Temp code for storing user creds, for refactoring.
        localStorage.setItem('CurrentUser', JSON.stringify(result.user));
        localStorage.setItem('CurrentUserRole', JSON.stringify(result.user.role));
        localStorage.setItem('Token', result.token);
        
        this.router.navigate(['/dashboard/analytics']);
        this.snackbar.open('Login successful', 'Welcome ' + logindto.email, { duration: 5000 });
      }
      else {
        this.snackbar.open('Login Failed', result.message, { duration: 5000 });
      }
    }, error => {
      this.snackbar.open('Login Failed', error.error.message, { duration: 5000 });
    });
  }

  toggleVisibility() {
    if (this.visible) {
      this.inputType = 'password';
      this.visible = false;
      this.cd.markForCheck();
    } else {
      this.inputType = 'text';
      this.visible = true;
      this.cd.markForCheck();
    }
  }

  loadLogin() {
    this.isLogin = true;
    this.isRegister = false;
  }

  loadRegister() {
    this.isRegister = true;
    this.isLogin = false;
  }
}
