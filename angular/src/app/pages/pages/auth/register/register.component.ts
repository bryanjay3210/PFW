import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { LocationService } from 'src/services/location.service';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { Customer, CustomerDTO, Location } from 'src/services/interfaces/models';
import { CustomerService } from 'src/services/customer.service';
//import { Customer } from 'src/services/interfaces/customer.model';
import { EmailService } from 'src/services/email.service';
import { RegisterUserDTO } from 'src/services/interfaces/auth/registeruserdto.model';
import { UserDTO } from 'src/services/interfaces/models';
import moment from 'moment';

@Component({
  selector: 'vex-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  animations: [
    fadeInUp400ms
  ]
})
export class RegisterComponent implements OnInit {

  form: UntypedFormGroup;
  inputType = 'password';
  visible = false;
  isCustomerUser = true;
  isAccept = false;

  customer: CustomerDTO;
  //customerList: Customer[];
  // locationList = {} as Location[];
  locationListFiltered = {} as Location[];

  constructor(
    private router: Router,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private snackbar: MatSnackBar,
    private authservice: AuthService,
    private locationService: LocationService,
    private customerService: CustomerService,
    private emailService: EmailService,
    private changeDetector: ChangeDetectorRef
  ) { }

  ngOnInit() {
    // this.locationService.getLocations().subscribe((result = {} as Location[]) => (this.locationList = result));

    this.form = this.fb.group({
      isCustomerUser: [true],
      accountNumber: [''],
      locationId: [-1],
      name: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', Validators.required],
      passwordConfirm: ['', Validators.required],
    });
    //this.initializeFormGroup(this.isCustomerUser);
  }

  initializeFormGroup(isCustomerUser: boolean) {
    if (isCustomerUser) {
      this.form = this.fb.group({
        isCustomerUser: [true],
        accountNumber: [''],
        locationId: [''],
        name: ['', Validators.required],
        email: ['', Validators.required],
        password: ['', Validators.required],
        passwordConfirm: ['', Validators.required],
      });
    }
    else {
      this.form = this.fb.group({
        isCustomerUser: [''],
        name: ['', Validators.required],
        email: ['', Validators.required],
        password: ['', Validators.required],
        passwordConfirm: ['', Validators.required],
      });
    }

    this.changeDetector.detectChanges();
  }

  send() {
    // NOTE: Add validation for Customer User fields
    if (!this.form.valid) {
      this.snackbar.open('Registration Failed', 'Please provide required fields.', { duration: 5000 });
      return;
    }

    if (!this.isAccept) {
      this.snackbar.open('Registration Failed', 'Please accept the terms and conditions.', { duration: 5000 });
      return;
    }

    if (this.form.value.password !== this.form.value.passwordConfirm) {
      this.snackbar.open('Registration Failed', 'Password and Confirm Password did not match.', { duration: 5000 });
      return;
    }

    if (this.form.value.isCustomerUser === true) {
      if (this.form.value.accountNumber === '' || this.form.value.accountNumber === '0') {
        this.snackbar.open('Registration Failed', 'Account Number is required.', { duration: 5000 });
        return;
      }

      if (this.form.value.locationId === '' || this.form.value.locationId === -1) {
        this.snackbar.open('Registration Failed', 'Location is required.', { duration: 5000 });
        return;
      }
    }

    const userdto = {} as UserDTO;
    if (this.isCustomerUser) {
      userdto.isCustomerUser = this.isCustomerUser;
      userdto.customerId = this.customer.id;
      userdto.locationId = this.form.value.locationId;
    }

    userdto.roleId = this.isCustomerUser ? 2 : 3; // Default Customer User Role (View Only) : Customer Service
    userdto.userName = this.form.value.name;
    userdto.email = this.form.value.email;
    userdto.password = this.form.value.password;
    userdto.createdBy = 'REGISTRATION';
    userdto.createdDate = moment(new Date());
    userdto.isActive = true;
    userdto.isActivated = false;
    userdto.isChangePasswordOnLogin = true;

    this.authservice.register(userdto).subscribe(result => {
      if (result.status === 200) {
        this.sendUserRegistrationEmail(userdto);

        this.router.navigate(['/']);
        this.snackbar.open('Registration successful', 'Email sent to administrator for approval. ', { duration: 5000 });
      }
      else {
        this.snackbar.open('Registration Failed', result.message, { duration: 5000 });
      }
    }, error => {
      this.snackbar.open('Registration Failed', error.error.message, { duration: 5000 });
    });
  }

  sendUserRegistrationEmail(userdto: UserDTO) {
    // Send Email --->>>

    let registeruserdto = new RegisterUserDTO();

    if (this.isCustomerUser) {
      let userLocation = this.locationListFiltered.find(l => l.id === userdto.locationId);
      registeruserdto.accountNumber = this.customer.accountNumber;
      registeruserdto.addressLine1 = userLocation.addressLine1;
      registeruserdto.addressLine2 = userLocation.addressLine2;
      registeruserdto.city = userLocation.city;
      registeruserdto.country = userLocation.country;
      registeruserdto.customerName = this.customer.customerName;
      registeruserdto.email = userLocation.email;
      registeruserdto.faxNumber = userLocation.faxNumber;
      registeruserdto.locationName = userLocation.locationName;
      registeruserdto.phoneNumber = userLocation.phoneNumber;
      registeruserdto.state = userLocation.state;
      registeruserdto.zipCode = userLocation.zipCode;
    }

    registeruserdto.roleId = this.isCustomerUser ? 2 : 3; // Default Customer User Role (View Only) : Customer Service
    registeruserdto.contactName = userdto.userName;
    registeruserdto.password = userdto.password;
    registeruserdto.userName = userdto.userName;

    this.emailService.sendUserRegistrationEmail(registeruserdto).subscribe(result => {
      if (result) {
        console.log("Email Sent.");
      }
      else {
        console.log("Email Not Sent.");
      }
    }, error => {
      console.log("Email Not Sent.");
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

  isCustomerUserChange($event) {
    this.isCustomerUser = $event.checked;
    this.initializeFormGroup(this.isCustomerUser);
  }

  isAcceptChange($event) {
    this.isAccept = $event.checked;
  }

  searchAccount($event) {
    this.customer = undefined;
    this.locationListFiltered = [];

    if ($event) {
      let accountNumber = $event.target.value ? Number($event.target.value) : 0;

      if (accountNumber === 0) {
        this.snackbar.open('Invalid Account Number: ', $event.target.value, { duration: 3000 });
        return;
      }

      if (isNaN(accountNumber)) {
        this.snackbar.open('Invalid Account Number: ', $event.target.value, { duration: 3000 });
        return;
      }

      this.customerService.getCustomerByAccountNumber(accountNumber).subscribe({
        next: (result) => {
          if (result) {
            this.customer = result;
            this.locationListFiltered = result.locations;
          }
        },
        error: (e) => {
          this.snackbar.open('ERROR: ', 'Customer with account number ' + accountNumber + ' not found!', { duration: 3000 });
          return;
        },
        complete: () => console.info('complete') 
      })

      // this.customer = this.cus customerList.find(c => c.accountNumber === accountNumber);
      // if (!this.customer) {
      //   this.snackbar.open('ERROR: ', 'Customer with account number ' + accountNumber + ' not found!', { duration: 3000 });
      //   return;
      // }

      //this.locationListFiltered = this.locationList.filter(l => l.customerId === this.customer.id);
      this.changeDetector.detectChanges();
    }
  }
}
