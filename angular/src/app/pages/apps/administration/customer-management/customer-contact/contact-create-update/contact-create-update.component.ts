import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { CustomerService } from 'src/services/customer.service';
import { LookupService } from 'src/services/lookup.service';
import { Contact, Location } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User } from 'src/services/interfaces/models';
import { LocationService } from 'src/services/location.service';
import { ContactService } from 'src/services/contact.service';
import { MatSlideToggle } from '@angular/material/slide-toggle';

@Component({
  selector: 'vex-contact-create-update',
  templateUrl: './contact-create-update.component.html',
  styleUrls: ['./contact-create-update.component.scss'],

})
export class ContactCreateUpdateComponent implements OnInit {
  @ViewChild('creditSlider', { static: false }) creditSld: MatSlideToggle;
  @ViewChild('orderSlider', { static: false }) orderSld: MatSlideToggle;
  @ViewChild('statementSlider', { static: false }) statementSld: MatSlideToggle;

  static id = 100;
  imageDefault = "https://icons.iconarchive.com/icons/treetog/junior/256/contacts-icon.png";

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  locationList = {} as Location[];
  positionTypeList: Lookup[];
  customerContactList = {} as Contact[];
  customerId: number;

  isEmailCreditMemo: boolean = false;
  isEmailOrder: boolean = false;
  isEmailStatement: boolean = false;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isFromOrder: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<ContactCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private locationService: LocationService,
    private lookupService: LookupService,
    private contactService: ContactService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit() {
    this.locationService.getLocationsByCustomerId((this.defaults && isNaN(this.defaults)) ? this.defaults.customerId : this.defaults).subscribe((result = {} as Location[]) => (this.locationList = result));
    this.lookupService.getPositionTypes().subscribe((result: Lookup[]) => (this.positionTypeList = result));

    if (this.defaults && isNaN(this.defaults)) {
      this.customerId = this.defaults.customerId;
      if (this.defaults.isFromOrder) {
        this.mode = 'create';
        this.isFromOrder = true;
      }
      else {
        this.mode = 'update';
        this.isEmailCreditMemo = this.defaults.isEmailCreditMemo;
        this.isEmailOrder = this.defaults.isEmailOrder;
        this.isEmailStatement = this.defaults.isEmailStatement;
      }
    } 
    else {
      this.mode = 'create';
      this.customerId = this.defaults;
      this.defaults = {} as Contact;
    }

    this.contactService.getContactsByCustomerId(this.customerId).subscribe((result: Contact[]) => (this.customerContactList = result));

    this.form = this.fb.group({
      id: [ContactCreateUpdateComponent.id++],
      contactName: [this.defaults.contactName || '', Validators.required],
      positionTypeId: [this.defaults.positionTypeId || undefined, Validators.required],
      locationId: [this.defaults.locationId || undefined, Validators.required],
      phoneNumber: [this.defaults.phoneNumber || '', Validators.required],
      email: [this.defaults.email || '', Validators.required],
      notes: this.defaults.notes
    });
  }

  save() {
    if (this.mode === 'create') {
      if (this.form.valid) {
        // Validate Email
        let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
        if (!regexp.test(this.form.value.email.toLowerCase())) {
          this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
          return;
        }
        this.alertService.createNotification("Contact").then(answer => {
          if (!answer.isConfirmed) return;
          if (!this.duplicateContact()) {
            this.createContact();
          }
          else this.alertService.duplicateNotification('Contact');
          
        });
      }
      else this.alertService.validationNotification("Contact");
    }
    else if (this.mode === 'update') {
      if (this.form.valid) {
        // Validate Email
        let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
        if (!regexp.test(this.form.value.email.toLowerCase())) {
          this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
          return;
        }
        this.alertService.updateNotification("Contact").then(answer => {
          if (!answer.isConfirmed) return;
          this.updateContact();
        });
      }
      else this.alertService.validationNotification("Contact");
    }
  }
  
  duplicateContact(): boolean {
    let result = false;
    // let contact = this.customerContactList.find(e => e.contactName.trim().toLowerCase() === this.form.value.contactName.trim().toLowerCase());
    // if (contact) {
    //   result = true;
    // }
    return result;
  }

  createContact() {
    const contact = {} as Contact;
    contact.customerId = this.customerId;
    contact.locationId = this.form.value.locationId;
    contact.positionTypeId = this.form.value.positionTypeId;
    contact.contactName = this.form.value.contactName;
    contact.phoneNumber = this.form.value.phoneNumber;
    contact.email = this.form.value.email;
    contact.notes = this.form.value.notes;
    contact.isActive = true;
    contact.isDeleted = false;
    contact.createdBy = "demo@user.com";
    contact.createdDate = moment(new Date());
    contact.isEmailCreditMemo = this.isEmailCreditMemo;
    contact.isEmailOrder = this.isEmailOrder;
    contact.isEmailStatement = this.isEmailStatement;
    this.dialogRef.close(contact);
  }

  updateContact() {
    let contact = this.defaults;
    contact.customerId = this.defaults.customerId;
    contact.locationId = this.form.value.locationId;
    contact.positionTypeId = this.form.value.positionTypeId;
    contact.contactName = this.form.value.contactName;
    contact.phoneNumber = this.form.value.phoneNumber;
    contact.email = this.form.value.email;
    contact.notes = this.form.value.notes;
    contact.isActive = this.defaults.isActive;
    contact.isDeleted = this.defaults.isDeleted;
    contact.createdBy = this.defaults.createdBy;
    contact.createdDate = this.defaults.createdDate;
    contact.modifiedBy = "modify@user.com";
    contact.modifiedDate = moment(new Date());
    contact.isEmailCreditMemo = this.isEmailCreditMemo;
    contact.isEmailOrder = this.isEmailOrder;
    contact.isEmailStatement = this.isEmailStatement;
    contact.id = this.defaults.id;

    this.dialogRef.close(contact);
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  setEmailCreditMemo(event: any) {
    if (event.checked) {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      if (!regexp.test(this.form.value.email.toLowerCase())) {
        this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
        this.isEmailCreditMemo = !event.checked;
        this.creditSld.checked = false;
        return;
      }
    }

    this.isEmailCreditMemo = event.checked;
  }

  setEmailOrder(event: any) {
    if (event.checked) {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      if (!regexp.test(this.form.value.email.toLowerCase())) {
        this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
        this.isEmailOrder = !event.checked;
        this.orderSld.checked = false;
        return;
      }
    }

    this.isEmailOrder = event.checked;
  }

  setEmailStatement(event: any) {
    if (event.checked) {
      let regexp = new RegExp("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$");
      if (!regexp.test(this.form.value.email.toLowerCase())) {
        this.alertService.requiredNotification('Contact email ' + this.form.value.email + ' is invalid!');
        this.isEmailStatement = !event.checked;
        this.statementSld.checked = false;
        return;
      }
    }

    this.isEmailStatement = event.checked;
  }
}
