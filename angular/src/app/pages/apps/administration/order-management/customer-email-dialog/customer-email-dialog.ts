import { AfterViewInit, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectChange } from '@angular/material/select';
import { UntilDestroy } from '@ngneat/until-destroy';
import moment from 'moment';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { AlertService } from 'src/services/alert.service';
import { ContactService } from 'src/services/contact.service';
import { CustomerService } from 'src/services/customer.service';
import { Contact, CustomerEmailDTO, OrderDetail, User, VendorCatalog } from 'src/services/interfaces/models';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Validators } from '@angular/forms';

@UntilDestroy()
@Component({
  selector: 'vex-customer-email-dialog',
  templateUrl: './customer-email-dialog.html',
  styleUrls: ['./customer-email-dialog.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class CustomerEmailDialog implements OnInit, AfterViewInit {
  @ViewChild('inputName') inputName!: ElementRef;
  @ViewChild('inputPhone') inputPhone!: ElementRef;
  @ViewChild('inputEmail') inputEmail!: ElementRef;
  
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  contactList: CustomerEmailDTO[] = [];
  customerId: number;

  form = new FormGroup({
    name: new FormControl(''),
    phone: new FormControl(''),
    email: new FormControl('',[Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")])
  });

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<CustomerEmailDialog>,
    private customerService: CustomerService,
    private contactService: ContactService,
    private alertService: AlertService
  ) {

    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit(): void {
    this.contactList = this.defaults.contacts;
    this.customerId = this.defaults.customerId;
  }

  ngAfterViewInit() {
    this.inputName.nativeElement.value = '';
    this.inputPhone.nativeElement.value = '';
    this.inputEmail.nativeElement.value = '';
  }

  setManualContact(name: string, phone: string, email: string) {
    if (this.inputName.nativeElement.value == '') {
      this.alertService.requiredNotification('Contact name is required!');
      return;
    }
    
    if (this.inputEmail.nativeElement.value == '') {
      this.alertService.requiredNotification('Contact email is required!');
      return;
    }
    else {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      if (!regexp.test(this.inputEmail.nativeElement.value.toLowerCase())) {
        this.alertService.requiredNotification('Contact email ' + this.inputEmail.nativeElement.value + ' is invalid!');
        return;
      }
    }

    this.alertService.createNotification("Contact").then(answer => {
      if (!answer.isConfirmed) return;
      if (!this.duplicateContact(email)) {
        this.createContact(name, phone, email);
      }
      else this.alertService.duplicateNotification('Contact');
    });
  }

  duplicateContact(email: string): boolean {
    let result = false;
    if (this.contactList.length > 0) {
      let contact = this.contactList.find(e => e.email.trim().toLowerCase() === email.trim().toLowerCase());
      if (contact) {
        result = true;
      }
    }
    return result;
  }

  createContact(name: string, phone: string, email: string) {
    const contact = {} as Contact;
    contact.customerId = this.customerId;
    contact.locationId = this.contactList && this.contactList.length > 0 ? this.contactList[0].locationId : undefined;
    contact.positionTypeId = this.contactList && this.contactList.length > 0 ? this.contactList[0].positionTypeId : undefined;
    contact.contactName = name;
    contact.phoneNumber = phone;
    contact.email = email;
    contact.notes = '';
    contact.isActive = true;
    contact.isDeleted = false;
    contact.createdBy = this.currentUser.userName;
    contact.createdDate = moment(new Date());
    
    this.contactService.createContact(contact).subscribe((result: Contact[]) => {
      if (result) {
        this.contactList = [];
        this.alertService.successNotification("Contact", "Create");
        
        this.customerService.getCustomerEmailsById(result[0].customerId).subscribe(result => {
          if (result && result.length > 0)
          {
            this.contactList = result;
            this.inputName.nativeElement.value = '';
            this.inputPhone.nativeElement.value = '';
            this.inputEmail.nativeElement.value = '';
          }
        });
      }
      else this.alertService.failNotification("Contact", "Create");
    });
  }

  rowClicked(row: CustomerEmailDTO) {
    let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
    if (!regexp.test(row.email.toLowerCase())) {
      this.alertService.requiredNotification('Contact email ' + row.email + ' is invalid!');
      return;
    }

    this.dialogRef.close(row);
  }
}