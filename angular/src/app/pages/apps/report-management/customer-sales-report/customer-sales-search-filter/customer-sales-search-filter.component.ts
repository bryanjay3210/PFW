import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { CustomerCredit, CustomerSalesFilterDTO, Payment, User } from 'src/services/interfaces/models';
import { AlertService } from 'src/services/alert.service';
import moment from 'moment';


@Component({
  selector: 'vex-customer-sales-search-filter',
  templateUrl: './customer-sales-search-filter.component.html',
  styleUrls: ['./customer-sales-search-filter.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class CustomerSalesSearchFilterComponent implements OnInit {
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";

  form: UntypedFormGroup;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<CustomerSalesSearchFilterComponent>,
    private fb: UntypedFormBuilder,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerSalesReport);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.form = this.fb.group({
      deliveryDate: [''],
    });

    this.initializeFormGroup();
  }

  initializeFormGroup() {
    this.form = this.fb.group({
      col1FrDate: [this.defaults && this.defaults.col1FrDate ? moment(this.defaults.col1FrDate).toDate() : ''],
      col1ToDate: [this.defaults && this.defaults.col1ToDate ? moment(this.defaults.col1ToDate).toDate() : ''],
      col2FrDate: [this.defaults && this.defaults.col2FrDate ? moment(this.defaults.col2FrDate).toDate() : ''],
      col2ToDate: [this.defaults && this.defaults.col2ToDate ? moment(this.defaults.col2ToDate).toDate() : ''],
      col3FrDate: [this.defaults && this.defaults.col3FrDate ? moment(this.defaults.col3FrDate).toDate() : ''],
      col3ToDate: [this.defaults && this.defaults.col3ToDate ? moment(this.defaults.col3ToDate).toDate() : ''],
      col4FrDate: [this.defaults && this.defaults.col4FrDate ? moment(this.defaults.col4FrDate).toDate() : ''],
      col4ToDate: [this.defaults && this.defaults.col4ToDate ? moment(this.defaults.col4ToDate).toDate() : ''],
      col5FrDate: [this.defaults && this.defaults.col5FrDate ? moment(this.defaults.col5FrDate).toDate() : ''],
      col5ToDate: [this.defaults && this.defaults.col5ToDate ? moment(this.defaults.col5ToDate).toDate() : ''],
      col6FrDate: [this.defaults && this.defaults.col6FrDate ? moment(this.defaults.col6FrDate).toDate() : ''],
      col6ToDate: [this.defaults && this.defaults.col6ToDate ? moment(this.defaults.col6ToDate).toDate() : ''],
      col7FrDate: [this.defaults && this.defaults.col7FrDate ? moment(this.defaults.col7FrDate).toDate() : ''],
      col7ToDate: [this.defaults && this.defaults.col7ToDate ? moment(this.defaults.col7ToDate).toDate() : ''],
      col8FrDate: [this.defaults && this.defaults.col8FrDate ? moment(this.defaults.col8FrDate).toDate() : ''],
      col8ToDate: [this.defaults && this.defaults.col8ToDate ? moment(this.defaults.col8ToDate).toDate() : ''],
      col9FrDate: [this.defaults && this.defaults.col9FrDate ? moment(this.defaults.col9FrDate).toDate() : ''],
      col9ToDate: [this.defaults && this.defaults.col9ToDate ? moment(this.defaults.col9ToDate).toDate() : ''],
      col10FrDate: [this.defaults && this.defaults.col10FrDate ? moment(this.defaults.col10FrDate).toDate() : ''],
      col10ToDate: [this.defaults && this.defaults.col10ToDate ? moment(this.defaults.col10ToDate).toDate() : ''],
      col11FrDate: [this.defaults && this.defaults.col11FrDate ? moment(this.defaults.col11FrDate).toDate() : ''],
      col11ToDate: [this.defaults && this.defaults.col11ToDate ? moment(this.defaults.col11ToDate).toDate() : ''],
      col12FrDate: [this.defaults && this.defaults.col12FrDate ? moment(this.defaults.col12FrDate).toDate() : ''],
      col12ToDate: [this.defaults && this.defaults.col12ToDate ? moment(this.defaults.col12ToDate).toDate() : ''],
    });
  }

  onDateChange(dateControl: string) {
    this.form.get(dateControl).setValue(''); 
  }

  applyCustomerSalesSearchFilter() {
    const filter = {} as CustomerSalesFilterDTO;
    filter.col1FrDate = this.form.value.col1FrDate === '' ? null : moment(new Date(this.form.value.col1FrDate).toISOString());
    filter.col1ToDate = this.form.value.col1ToDate === '' ? null : moment(new Date(this.form.value.col1ToDate).toISOString());
    filter.col2FrDate = this.form.value.col2FrDate === '' ? null : moment(new Date(this.form.value.col2FrDate).toISOString());
    filter.col2ToDate = this.form.value.col2ToDate === '' ? null : moment(new Date(this.form.value.col2ToDate).toISOString());
    filter.col3FrDate = this.form.value.col3FrDate === '' ? null : moment(new Date(this.form.value.col3FrDate).toISOString());
    filter.col3ToDate = this.form.value.col3ToDate === '' ? null : moment(new Date(this.form.value.col3ToDate).toISOString());
    filter.col4FrDate = this.form.value.col4FrDate === '' ? null : moment(new Date(this.form.value.col4FrDate).toISOString());
    filter.col4ToDate = this.form.value.col4ToDate === '' ? null : moment(new Date(this.form.value.col4ToDate).toISOString());
    filter.col5FrDate = this.form.value.col5FrDate === '' ? null : moment(new Date(this.form.value.col5FrDate).toISOString());
    filter.col5ToDate = this.form.value.col5ToDate === '' ? null : moment(new Date(this.form.value.col5ToDate).toISOString());
    filter.col6FrDate = this.form.value.col6FrDate === '' ? null : moment(new Date(this.form.value.col6FrDate).toISOString());
    filter.col6ToDate = this.form.value.col6ToDate === '' ? null : moment(new Date(this.form.value.col6ToDate).toISOString());
    filter.col7FrDate = this.form.value.col7FrDate === '' ? null : moment(new Date(this.form.value.col7FrDate).toISOString());
    filter.col7ToDate = this.form.value.col7ToDate === '' ? null : moment(new Date(this.form.value.col7ToDate).toISOString());
    filter.col8FrDate = this.form.value.col8FrDate === '' ? null : moment(new Date(this.form.value.col8FrDate).toISOString());
    filter.col8ToDate = this.form.value.col8ToDate === '' ? null : moment(new Date(this.form.value.col8ToDate).toISOString());
    filter.col9FrDate = this.form.value.col9FrDate === '' ? null : moment(new Date(this.form.value.col9FrDate).toISOString());
    filter.col9ToDate = this.form.value.col9ToDate === '' ? null : moment(new Date(this.form.value.col9ToDate).toISOString());
    filter.col10FrDate = this.form.value.col10FrDate === '' ? null : moment(new Date(this.form.value.col10FrDate).toISOString());
    filter.col10ToDate = this.form.value.col10ToDate === '' ? null : moment(new Date(this.form.value.col10ToDate).toISOString());
    filter.col11FrDate = this.form.value.col11FrDate === '' ? null : moment(new Date(this.form.value.col11FrDate).toISOString());
    filter.col11ToDate = this.form.value.col11ToDate === '' ? null : moment(new Date(this.form.value.col11ToDate).toISOString());
    filter.col12FrDate = this.form.value.col12FrDate === '' ? null : moment(new Date(this.form.value.col12FrDate).toISOString());
    filter.col12ToDate = this.form.value.col12ToDate === '' ? null : moment(new Date(this.form.value.col12ToDate).toISOString());
    
    if (this.validationFailed(filter)) {
      this.alertService.requiredNotification('Unable to proceed. Please provide missing To Date filter.');
      return;
    } 

    this.dialogRef.close(filter);
  }
  
  validationFailed(filter: CustomerSalesFilterDTO) {
    if (filter.col1FrDate && !filter.col1ToDate) return true;
    if (filter.col2FrDate && !filter.col2ToDate) return true;
    if (filter.col3FrDate && !filter.col3ToDate) return true;
    if (filter.col4FrDate && !filter.col4ToDate) return true;
    if (filter.col5FrDate && !filter.col5ToDate) return true;
    if (filter.col6FrDate && !filter.col6ToDate) return true;
    if (filter.col7FrDate && !filter.col7ToDate) return true;
    if (filter.col8FrDate && !filter.col8ToDate) return true;
    if (filter.col9FrDate && !filter.col9ToDate) return true;
    if (filter.col10FrDate && !filter.col10ToDate) return true;
    if (filter.col11FrDate && !filter.col11ToDate) return true;
    if (filter.col12FrDate && !filter.col12ToDate) return true;
    return false;
  }
}