import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { CustomerCredit, Payment, User } from 'src/services/interfaces/models';
import { AlertService } from 'src/services/alert.service';
import moment from 'moment';


@Component({
  selector: 'vex-back-order',
  templateUrl: './back-order.component.html',
  styleUrls: ['./back-order.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class BackOrderComponent implements OnInit {
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Payment,
    private dialogRef: MatDialogRef<BackOrderComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PaymentManagement);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.form = this.fb.group({
      deliveryDate: [''],
    });
  }

  saveBackOrder() {
    this.dialogRef.close(this.form.value.deliveryDate);
  }
}