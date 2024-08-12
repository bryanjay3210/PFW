import { AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { VendorService } from 'src/services/vendor.service';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Vendor, User, DriverLog, DriverLogPaginatedListDTO } from 'src/services/interfaces/models';
import moment from 'moment';
import { DriverLogCreateUpdateComponent } from './driver-log-create-update/driver-log-create-update.component';
import { DriverLogService } from 'src/services/driverlog.service';
import { EmailService } from 'src/services/email.service';

@UntilDestroy()
@Component({
  selector: 'vex-driver-log-table',
  templateUrl: './driver-log-table.component.html',
  styleUrls: ['./driver-log-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
  providers: [
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'standard'
      } as MatFormFieldDefaultOptions
    }
  ]
})

export class DriverLogTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<DriverLog>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Driver Log Number', property: 'driverLogNumber', type: 'text', visible: true },
    { label: 'Driver Name', property: 'driverName', type: 'text', visible: true },
    { label: 'Driver Log Date', property: 'createdDate', type: 'text', visible: true },
    { label: 'Status', property: 'statusDetail', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');
  pageSize: number = 50;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  dataSource: MatTableDataSource<DriverLog> | null;
  selection = new SelectionModel<DriverLog>(true, []);
  searchCtrl = new UntypedFormControl();
  fromDateCtrl = new UntypedFormControl();
  toDateCtrl = new UntypedFormControl();

  
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;
  data: any;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private driverLogService: DriverLogService,
    private emailService: EmailService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.DriverLog);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnDestroy() {
    location.reload();
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.dataSource = new MatTableDataSource();
    this.getData();
    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    
    // let styleElement = document.getElementById('section-print');
    // styleElement.append('@media print { @page { size: A4 landscape; } }')
  }

  getData() {
    this.getPaginatedDriverLogsList();
  }

  getPaginatedDriverLogsList() {
    this.alertService.showBlockUI('Loading Driver Logs...');
    if (!!this.search) this.search = this.search.trim();
    this.driverLogService.getDriverLogsPaginated(this.pageSize, this.pageIndex, "DriverLogNumber", "DESC", this.search).subscribe((result: DriverLogPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }
    });
  }

  getPaginatedDriverLogsListByDate() {
    this.alertService.showBlockUI('Loading Driver Logs...');
    // let frDate = moment(new Date(this.fromDateCtrl.value)).format('MM/DD/YYYY ');
    // let toDate = moment(new Date(this.toDateCtrl.value)).format('MM/DD/YYYY');

    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.driverLogService.getDriverLogsByDatePaginated(this.pageSize, this.pageIndex, frDate, toDate).subscribe((result: DriverLogPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }
    });
  }
  
  clearDateSearch() {
    this.searchCtrl.setValue('');
    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    this.getPaginatedDriverLogsList();
  }
  
  createDriverLog() {
    this.dialog.open(DriverLogCreateUpdateComponent, {
      height: '80%',
      width: '100%',
    }).afterClosed().subscribe((driverLog: DriverLog) => {
      if (driverLog) {
        this.driverLogService.createDriverLog(driverLog).subscribe((result) => {
          if (result) {
            this.getPaginatedDriverLogsList();
            this.alertService.successNotification("Driver Log", "Create");
            this.emailService.sendDriverLogEmails(driverLog).subscribe(result => {});
          }
          else {
            this.alertService.failNotification("Driver Log", "Create");
          }
        });
      }
    });
  }

  updateDriverLog(driverLog: DriverLog) {
    this.dialog.open(DriverLogCreateUpdateComponent, {
      height: '80%',
      width: '100%',
      data: driverLog
    }).afterClosed().subscribe((updateDriverLog: DriverLog) => {
      if (updateDriverLog) {
        this.driverLogService.updateDriverLog(updateDriverLog).subscribe((result) => {
          if (result) {
            this.alertService.successNotification("Driver Log", "Update");
          }
          else {
            this.alertService.failNotification("Driver Log", "Update");
          }

          this.getPaginatedDriverLogsList();
        });
      }
      else this.getPaginatedDriverLogsList();
    });
  }

  printDriverLog(event: any, row: DriverLog) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => {
      window.print();
    }, 2000);
  }

  mapRowTodata(row: DriverLog): any {
    const result = {} as DriverLogResult;
    
    result.date = moment(row.createdDate).format('MM/DD/YYYY'); //moment(new Date(row.createdDate)).format('MM/DD/YYYY');
    result.time = moment(row.createdDate).format('h:mm A');
    result.driverName = row.driverName;
    result.driverLogNumber = row.driverLogNumber;
    result.stops = 0;
    result.lineItems = [];
    let on = '';
    let st = '';
    row.driverLogDetails.forEach(e => {
      let tempON = e.orderNumber.toString();
      let tempST = e.shipAddressName;
      const lineItem = {} as LineItem;
      
      if (on !== tempON) {
        on = tempON;
        lineItem.orderNumber = on;
      }
      else {
        lineItem.orderNumber = '';
      }
      
      if (st !== tempST) {
        st = tempST;
        lineItem.shipTo = st;
        result.stops += 1;
      }
      else {
        lineItem.shipTo = '';
      }
      
      lineItem.partNumber = e.partNumber;
      lineItem.quantity =  e.orderQuantity.toString();
      lineItem.terms = e.paymentTermName;
      lineItem.totalAmount = this.formatCurrencyBlank(e.orderTotalAmount);
      result.lineItems.push(lineItem);
    });

    return result;
  }

  formatDateOnly(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY');
  }

  deleteDriverLog(event: any, row: DriverLog) {
    if (event) {
      event.stopPropagation();
    }
  }

  deleteDriverLogs(inventories: DriverLog[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deleteDriverLog(inventories).subscribe((result: DriverLog[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deleteDriverLog(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedDriverLogs();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedDriverLogsList();
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    if (this.fromDateCtrl.value !== '' && this.toDateCtrl.value !== '') {
      this.getPaginatedDriverLogsListByDate();
    }
    else {
      this.getPaginatedDriverLogsList();
    }
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchDriverLogs() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedDriverLogsList();
  }

  searchDriverLogsByDate() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedDriverLogsListByDate();
  }

  showInactiveVendors() {
    this.getData();
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  formatCurrencyBlank(amount: number) {
    return (amount) ? amount.toFixed(2) : '';
  }

  onDateChange() {
    this.searchCtrl.setValue('');
    this.toDateCtrl.setValue('');
  }
}

export interface DriverLogResult {
    date: string;
    time: string;
    driverName: string;
    driverLogNumber: string;
    stops: number;
    lineItems: LineItem[];
}

export interface LineItem {
  orderNumber: string;
  shipTo: string,
  quantity: string,
  partNumber: string,
  terms: string;
  totalAmount: string;
  payment: string;
}
