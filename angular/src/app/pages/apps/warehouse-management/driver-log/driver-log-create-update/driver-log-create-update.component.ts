import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import moment from 'moment';
import { AlertService } from 'src/services/alert.service';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { Order, DriverLogDetail, DriverLog, Role, User, Vendor, StockOrderDetailDTO, Driver } from 'src/services/interfaces/models';
import { Lookup } from 'src/services/interfaces/lookup.model';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { SelectionModel } from '@angular/cdk/collections';
import { DriverLogService } from 'src/services/driverlog.service';
import { DriverService } from 'src/services/driver.service';
import { error } from 'console';
import { OrderService } from 'src/services/order.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { DriverLogDetailService } from 'src/services/driverlogdetail.service';

@Component({
  selector: 'vex-driver-log-create-update',
  templateUrl: './driver-log-create-update.component.html',
  styleUrls: ['./driver-log-create-update.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class DriverLogCreateUpdateComponent implements OnInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('driver', { static: false }) driver: ElementRef;
  @ViewChild('order', { static: false }) order: ElementRef;

  columns: TableColumn<any>[] = [];
  static id = 100;
  imageDefault = "assets/img/pfw_logo_sm.png";
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<any> | null;

  selection = new SelectionModel<DriverLogDetail>(true, []);
  roleList: Role[];
  userTypeList: Lookup[];
  accessTypeList: Lookup[];
  
  currentDriver: Driver = undefined;
  currentOrder: Order = undefined;
  currentDriverLog: DriverLog = undefined;

  orderList: Order[] = [];

  todayDate: Date = new Date();
  visible = false;
  isUseCredit: boolean = false;
  isCashOrCreditSelected: boolean = true;
 
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  currentVendor: Vendor = undefined;
  creditBalance: number = 0;
  totalAmountCM: number = 0;

  driverCtrl = new UntypedFormControl();
  orderCtrl = new UntypedFormControl();
  currentOrderNumber: number = 0;
  currentShipAddressName: string = '';
  currentPaymentTermName: string = '';

  totalStops: number = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: DriverLog,
    private dialogRef: MatDialogRef<DriverLogCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private cd: ChangeDetectorRef,
    private driverService: DriverService,
    private driverLogDetailService: DriverLogDetailService,
    private orderService: OrderService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.DriverLog);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource();
    this.currentOrderNumber = 0;
    this.currentShipAddressName = '';
    this.currentPaymentTermName = '';
    if (this.defaults) {
      this.mode = 'update';
      this.currentDriverLog = this.defaults;
      this.setTableColumns();
      this.dataSource.data = this.defaults.driverLogDetails;
      this.getTotalStops();
    } else {
      this.mode = 'create';
      this.setTableColumns();
      this.dataSource.data = [];
      this.defaults = {} as DriverLog;
    }

    this.form = this.fb.group({
      id: [DriverLogCreateUpdateComponent.id++],
      driverLogNumber: [(this.defaults.driverLogNumber) || ''],
    });
  }

  setTableColumns() {
    if (this.isUpdateMode()) {
      this.columns = [
        { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
        { label: 'Order Number', property: 'orderNumber', type: 'text', visible: true},
        { label: 'Ship To', property: 'shipAddressName', type: 'text', visible: true},
        { label: 'Order Quantity', property: 'orderQuantity', type: 'text', visible: true },
        { label: 'PartNumber', property: 'partNumber', type: 'text', visible: true },
        { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
        { label: 'Amount', property: 'totalAmount', type: 'number', visible: true },
        { label: 'Total Amount', property: 'orderTotalAmount', type: 'number', visible: true },
        { label: 'Status', property: 'statusDetail', type: 'text', visible: true },
        { label: 'Actions', property: 'actions', type: 'button', visible: true }
      ];
    }
    else if(this.isCreateMode()) {
      this.columns = [
        // { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
        { label: 'Order #', property: 'orderNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
        { label: 'Invoice #', property: 'invoiceNumber', type: 'text', visible: true },
        { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
        { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
        { label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true },
        { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
        { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
        { label: 'Customer', property: 'customerName', type: 'text', visible: true },
        { label: 'Account #', property: 'accountNumber', type: 'text', visible: true },
        { label: 'Phone #', property: 'phoneNumber', type: 'text', visible: true },
        { label: 'Shipping To', property: 'shipAddressName', type: 'text', visible: true },
        { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
        { label: 'Actions', property: 'actions', type: 'button', visible: true },
      ];
    }
  }

  searchDriver() {
    // if (this.driverCtrl.value === '') {
    //   this.driver.nativeElement.focus();
    //   return;
    // }
    this.driverService.getDriverByDriverNumber(this.driverCtrl.value).subscribe((result: Driver) => {
      if (result) {
        //this.resetResults();
        this.currentDriver = result;
        this.addDriverLog();

        this.order.nativeElement.focus();
      }
      else {
        this.alertService.failNotification('Driver', 'Search');
        this.driver.nativeElement.focus();
      }
    },
    error => {
      this.alertService.failNotification('Driver', 'Search');
      this.driver.nativeElement.focus();
    });
  }

  searchOrder() {
    // if (this.orderCtrl.value === '') {
    //   this.order.nativeElement.focus();
    //   return;
    // }
    this.orderService.getOrderByOrderNumber(this.orderCtrl.value).subscribe((result: Order) => {
      if (result) {
        // Check if product is already in list
        let exist = this.orderList.find(e => e.id === result.id);
        if (!exist) {
          this.orderList.push(result);
          this.dataSource.data = this.orderList;
          this.orderCtrl.setValue('');
          this.cd.detectChanges();
  
          this.currentOrder = result;
          this.addDriverLogDetail();
        }
        else {
          this.alertService.existNotification('Order');
          this.orderCtrl.setValue('');
          this.cd.detectChanges();
        }
      }
      else {
        this.orderCtrl.setValue('');
        this.cd.detectChanges();
        this.alertService.failNotificationSearchDriverLogOrder('Order', 'Search');
      }
    },
    error => {
      this.orderCtrl.setValue('');
      this.cd.detectChanges();
      this.alertService.failNotificationSearchDriverLogOrder('Order', 'Search');
    });
  }

  addDriverLog() {
    var driverLog = {} as DriverLog;
    driverLog.createdBy = this.currentUser.userName;
    driverLog.createdDate = moment(new Date());
    driverLog.driverId = this.currentDriver.id;
    driverLog.driverLogNumber = '';
    driverLog.driverName = this.currentDriver.firstName + ' ' + this.currentDriver.lastName;
    driverLog.id = 0;
    driverLog.isActive = true;
    driverLog.isDeleted = false;
    driverLog.statusDetail = 'Delivered';
    driverLog.statusId = 1;
    this.currentDriverLog = driverLog;
    this.currentDriverLog.driverLogDetails = [];
  }

  addDriverLogDetail() {
    // let orderDetailId = 0;
    this.currentOrder.orderDetails.forEach(e => {
      const driverLogDetail = {} as DriverLogDetail;
      // if (orderDetailId === 0) {
      //   driverLogDetail.orderTotalAmount = this.currentOrder.totalAmount;
      //   orderDetailId = e.id;
      // }
      driverLogDetail.createdBy = this.currentDriverLog.createdBy;
      driverLogDetail.createdDate = this.currentDriverLog.createdDate;
      driverLogDetail.driverLogId = 0;
      driverLogDetail.id = 0;
      driverLogDetail.isActive = true;
      driverLogDetail.isDeleted = false;
      driverLogDetail.orderDetailId = e.id;
      driverLogDetail.orderId = this.currentOrder.id;
      driverLogDetail.orderNumber = this.currentOrder.orderNumber;
      driverLogDetail.orderQuantity = e.orderQuantity;
      driverLogDetail.partNumber = e.partNumber;
      driverLogDetail.paymentTermName = this.currentOrder.paymentTermName;
      driverLogDetail.shipAddressName = this.currentOrder.shipAddressName;
      driverLogDetail.statusDetail = this.currentDriverLog.statusDetail;
      driverLogDetail.statusId = this.currentDriverLog.statusId;
      driverLogDetail.totalAmount = e.totalAmount;
      this.currentDriverLog.driverLogDetails.push(driverLogDetail);
    });

    this.cd.detectChanges();
  }

  clearOrders() {
    this.alertService.clearNotification("Orders").then(answer => {
      if (!answer.isConfirmed) return;
      this.resetResults();
      //this.alertService.successNotification('Orders', 'Cleared');
    });
  }

  private resetResults() {
    this.driverCtrl.setValue('');
    this.orderCtrl.setValue('');
    this.orderList = [];
    this.dataSource.data = this.orderList;
    this.cd.detectChanges();
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ? this.selection.clear() : this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectDriverLogDetail(driverLogDetail: DriverLogDetail) {
    this.selection.toggle(driverLogDetail);
  }

  save() {
    if (this.isCreateMode()) {
      if (this.form.valid) {
        this.alertService.createNotification("Driver Log").then(answer => {
          if (!answer.isConfirmed) return;
            this.createDriverLog();
        });
      }
      else this.alertService.validationNotification("Driver Log");
    }
    else if (this.isUpdateMode()) {
      if (this.form.valid) {
        this.alertService.updateNotification("Driver Log").then(answer => {
          if (!answer.isConfirmed) return;
            this.updateDriverLog();
        });
      }
      else this.alertService.validationNotification("Driver Log");
    }
  }

  createDriverLog() {
    // const driverLog = {} as DriverLog;
    // this.mapFormValuesToDriverLog(driverLog);
    this.dialogRef.close(this.currentDriverLog);
  }

  updateDriverLog() {
    const driverLog = {} as DriverLog;
    this.mapFormValuesToDriverLog(driverLog);
    this.dialogRef.close(this.currentDriverLog);
  }

  mapFormValuesToDriverLog(driverLog: DriverLog) {
    driverLog = this.currentDriverLog;
  }

  printDriverLog(event: any, row: DriverLog) {
    if (event) {
      event.stopPropagation();
    }
    // this.data = this.mapRowTodata(row);
    // this.cd.detectChanges();
    // setTimeout( () => { 
    //   window.print(); 
    // }, 2000);
  }

  retry(event: any, row: DriverLogDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.actionNotification('Retry', 'Driver Log Detail').then(answer => {
      if (!answer.isConfirmed) return;

      row.statusId = 2;
      row.statusDetail = 'Retry';
      row.modifiedBy = this.currentUser.userName;
      row.modifiedDate = moment(new Date());

      this.driverLogDetailService.updateDriverLogDetail(row).subscribe(result => {
        if (result) {
          this.alertService.successNotification('Driver Log Detail', 'Update');
        }
        else this.alertService.failNotification('Driver Log Detail', 'Update');
      },
      error => {
        this.alertService.failNotification('Driver Log Detail', 'Update');
      });
    });
  }

  deleteOrder(event: any, row: Order) {
    if (event) {
      event.preventDefault();
    }
    this.alertService.deleteNotification('Order').then(answer => {
      if (!answer.isConfirmed) return;

      let index = this.orderList.findIndex(e => e.id === row.id); 
      this.orderList.splice(index, 1);
      this.dataSource.data = this.orderList;

      this.currentDriverLog.driverLogDetails = this.currentDriverLog.driverLogDetails.filter(e => e.orderId !== row.id);
      this.cd.detectChanges();
    });
  }

  deleteDriverLogDetail(event: any, row: DriverLogDetail) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.deleteNotification(this.isCreateMode() ? 'Order' : 'Driver Log Detail').then(answer => {
      if (!answer.isConfirmed) return;

      if (this.isCreateMode()) {
        // Remove Order from OrderList
        let index = this.orderList.findIndex(e => e.id === row.id); 
        this.orderList.splice(index, 1);
        this.dataSource.data = this.orderList;
      }
      else {
      //   this.driverLogService.softDeleteDriverLogDetail(row).subscribe(result => {
      //     if (result) {
      //       let tempData = this.dataSource.data;
      //       tempData.splice(this.dataSource.data.findIndex(e => e.id === row.id), 1);
      //       this.dataSource.data = tempData;
      //       this.alertService.successNotification("Driver Log Detail", "Delete");
      //       this.cd.detectChanges();
      //     }
      //     else {
      //       this.alertService.failNotification("Driver Log Detail", "Delete");
      //     }
      
       //});
      }
    });
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '';
  }

  getTotalStops() {
    this.totalStops = 0;
    let tempCustomer = '';
    this.dataSource.data.forEach(e => {
      if (tempCustomer !== e.shipAddressName) {
        tempCustomer = e.shipAddressName;
        this.totalStops += 1;
      }
    });
  }
}