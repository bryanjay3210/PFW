import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { CustomerCredit, CustomerDTO, Order, OrderDetail, Payment, PaymentDetail, PaymentTerm, PriceLevel, Role, User } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { AlertService } from 'src/services/alert.service';
import { CustomerCreditService } from 'src/services/customercredit.service';
import moment from 'moment';


@Component({
  selector: 'vex-payment-add-credit',
  templateUrl: './payment-add-credit.component.html',
  styleUrls: ['./payment-add-credit.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class PaymentAddCreditComponent implements OnInit {
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentCustomer: CustomerDTO = undefined;
  creditBalance: number = 0;

  creditTypelList = [
    { id: 2, description: 'Return'},
    { id: 3, description: 'Manual'},
    { id: 4, description: 'Discount'},
    { id: 5, description: 'Others'},
  ]

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Payment,
    private dialogRef: MatDialogRef<PaymentAddCreditComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PaymentManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [PaymentAddCreditComponent.id++],
      amountPosted: [0],
      creditType: [0],
      creditReason: [''],
    });
  }

  addCredit() {
    if (this.form.valid) {
      if (Number(this.form.value.amountPosted) === 0) {
        this.alertService.validationFailedNotification('Amount Error', 'Amount must be greater than zero!');
        return;
      }
      this.alertService.createNotification("Customer Credit").then(answer => {
        if (!answer.isConfirmed) return;
        this.createCustomerCredit();
      });
    }
  }

  createCustomerCredit() {
    const customerCredit = {} as CustomerCredit;
    customerCredit.amountPosted = this.form.value.amountPosted;
    customerCredit.amountPostedDate = moment(new Date());
    customerCredit.createdBy = this.currentUser.userName;
    customerCredit.createdDate = customerCredit.amountPostedDate;
    customerCredit.creditReason = this.form.value.creditReason;
    customerCredit.creditType = this.form.value.creditType;
    customerCredit.referenceId = 10001;
    customerCredit.isActive = true;
    customerCredit.isDeleted = false;
    this.dialogRef.close(customerCredit);
  }
}