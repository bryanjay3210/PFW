import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { AuthService } from 'src/services/auth.service';
import { LoginDTO } from 'src/services/interfaces/auth/logindto.model';
import { RoleService } from 'src/services/role.service';

@Component({
  selector: 'vex-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    fadeInUp400ms
  ]
})
export class LoginComponent implements OnInit {

  form: UntypedFormGroup;
  inputType = 'password';
  visible = false;

  constructor(
    private router: Router,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private snackbar: MatSnackBar,
    private authservice: AuthService,
    private roleService: RoleService) { 
      this.clearLocalStorage();
    }

  ngOnInit() {
    this.clearLocalStorage();
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
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
        
        if (result.user.isCustomerUser === true) {
          this.router.navigate(['/apps/external-customer/customer-order-management']);
        }
        else {
          this.router.navigate(['/dashboard/analytics']);
        }
        
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
}
