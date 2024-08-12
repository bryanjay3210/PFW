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
import { Vendor, User, PurchaseOrder, PurchaseOrderPaginatedListDTO, DailyVendorSalesSummaryDTO, TotalVendorSalesDTO } from 'src/services/interfaces/models';
import moment from 'moment';
import { PurchaseOrderService } from 'src/services/purchaseorder.service';
import { PurchaseOrderCreateUpdateComponent } from './purchase-order-create-update/purchase-order-create-update.component';
//import { VendorCreateUpdateComponent } from './vendor-create-update/vendor-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-purchase-order-table',
  templateUrl: './purchase-order-table.component.html',
  styleUrls: ['./purchase-order-table.component.scss'],
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

export class PurchaseOrderTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<PurchaseOrder>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Vendor Name', property: 'vendorName', type: 'text', visible: true },
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true },
    { label: 'Vendor PO', property: 'vendorPO', type: 'text', visible: true },
    { label: 'PFW BNumber', property: 'pfwbNumber', type: 'text', visible: true },
    { label: 'Purchase Order Date', property: 'purchaseOrderDate', type: 'text', visible: true },
    { label: 'Status', property: 'poStatus', type: 'text', visible: true },
    { label: 'Modified Date', property: 'modifiedDate', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
    { label: 'Total Quantity', property: 'totalQuantity', type: 'text', visible: true },
    { label: 'User', property: 'createdBy', type: 'text', visible: true },
    { label: 'Received Date', property: 'receivedDate', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  dataSource: MatTableDataSource<PurchaseOrder> | null;
  selection = new SelectionModel<PurchaseOrder>(true, []);
  searchCtrl = new UntypedFormControl();
  fromDateCtrl = new UntypedFormControl();
  toDateCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;
  data: any;
  totalVendorSalesSummary: TotalVendorSalesDTO[];
  totalVendorSales: any;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private purchaseOrderService: PurchaseOrderService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PurchaseOrderManagement);
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

    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    
    this.dataSource = new MatTableDataSource();
    this.getData();
    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.getPaginatedPurchaseOrdersList();
  }

  getPaginatedPurchaseOrdersList() {
    this.alertService.showBlockUI('Loading Purchase Orders...');
    if (!!this.search) this.search = this.search.trim();
    this.purchaseOrderService.getPurchaseOrdersPaginated(this.pageSize, this.pageIndex, "PFWBNumber", "DESC", this.search).subscribe((result: PurchaseOrderPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }

      this.getDailyVendorSalesSummary();
    });
  }

  createPurchaseOrder() {
    this.dialog.open(PurchaseOrderCreateUpdateComponent, {
      height: '80%',
      width: '100%',
    }).afterClosed().subscribe((purchaseOrder: PurchaseOrder) => {
      if (purchaseOrder) {
        this.purchaseOrderService.createPurchaseOrder(purchaseOrder).subscribe((result: boolean) => {
          if (result) {
            this.getPaginatedPurchaseOrdersList();
            this.alertService.successNotification("Purchase Order", "Create");
          }
          else {
            this.alertService.failNotification("Purchase Order", "Create");
          }
        });
      }
    });
  }

  updatePurchaseOrder(purchaseOrder: PurchaseOrder) {
    this.dialog.open(PurchaseOrderCreateUpdateComponent, {
      height: '80%',
      width: '100%',
      data: purchaseOrder
    }).afterClosed().subscribe((updatePurchaseOrder: PurchaseOrder) => {
      if (updatePurchaseOrder) {
        this.purchaseOrderService.updatePurchaseOrder(updatePurchaseOrder).subscribe((result: boolean) => {
          if (result) {
            this.alertService.successNotification("Purchase Order", "Update");
          }
          else {
            this.alertService.failNotification("Purchase Order", "Update");
          }
          this.getPaginatedPurchaseOrdersList();
        });
      }
      else this.getPaginatedPurchaseOrdersList();
    });
  }

  printPurchaseOrder(event: any, row: PurchaseOrder) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => {
      window.print();
    }, 2000);
  }

  mapRowTodata(row: PurchaseOrder): any {
    let result: LineItem[] = [];
    row.purchaseOrderDetails.forEach(e => {
      const lineItem = {} as LineItem;
      lineItem.customerName = e.customerName;
      lineItem.description = e.partDescription;
      lineItem.pNum =  e.partNumber;
      lineItem.partsLink = e.mainPartsLinkNumber;
      lineItem.vendorCode = row.vendorCode;
      lineItem.vendorPartNumber = e.vendorPartNumber;
      lineItem.zone = e.shipZone;
      lineItem.poNumber = e.purchaseOrderNumber;
      lineItem.poDate = this.formatDateOnly(row.purchaseOrderDate);
      lineItem.deliveryType = e.deliveryMethod;
      lineItem.orderNumber = e.orderNumber.toString();
      lineItem.pfwBNumber = row.pfwbNumber;
      lineItem.deliveryDate = this.formatDateOnly(e.deliveryDate);
      lineItem.deliveryRoute = e.deliveryRoute === 1 ? 'AM' : 'PM';
      result.push(lineItem);
    });

    return result;
  }

  formatDateOnly(orderDate: moment.Moment) {
    return orderDate ? moment(orderDate).format('MM/DD/YYYY') : '';
  }

  deletePurchaseOrder(event: any, row: PurchaseOrder) {
    if (event) {
      event.stopPropagation();
    }
  }

  deletePurchaseOrders(inventories: PurchaseOrder[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deletePurchaseOrder(inventories).subscribe((result: PurchaseOrder[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deletePurchaseOrder(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedPurchaseOrders();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedPurchaseOrdersList();
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
      this.getPaginatedPurchaseOrdersListByDate();
    }
    else {
      this.getPaginatedPurchaseOrdersList();
    }
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchPurchaseOrders() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPurchaseOrdersList();
  }

  searchPurchaseOrdersByDate() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPurchaseOrdersListByDate();
  }

  getPaginatedPurchaseOrdersListByDate() {
    this.alertService.showBlockUI('Loading Purchase Orders...');
    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.purchaseOrderService.getPurchaseOrdersByDatePaginated(this.pageSize, this.pageIndex, frDate, toDate).subscribe((result: PurchaseOrderPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }

      this.getDailyVendorSalesSummaryByDate();
    });
  }

  getDailyVendorSalesSummary() {
    this.totalVendorSalesSummary = [];
    this.totalVendorSales = 0;
    let rawDate = new Date().setHours(0,0,0,0);
    let currentDate = new Date(rawDate).toISOString();

    this.purchaseOrderService.getDailySalesSummary(currentDate).subscribe(result => {
      if (result && result.vendorSummary.length > 0) {
        this.totalVendorSalesSummary = result.vendorSummary;
        let totalSales = this.totalVendorSalesSummary.map(e => e.salesAmount).reduce(function(a, b){ return a + b; });
        this.totalVendorSales = this.formatCurrency(totalSales);
        this.cd.detectChanges();
      }
    });
  }

  getDailyVendorSalesSummaryByDate() {
    this.totalVendorSalesSummary = [];
    this.totalVendorSales = 0;
    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.purchaseOrderService.getDailySalesSummaryByDate(frDate, toDate).subscribe(result => {
      if (result && result.vendorSummary.length > 0) {
        this.totalVendorSalesSummary = result.vendorSummary;
        let totalSales = this.totalVendorSalesSummary.map(e => e.salesAmount).reduce(function(a, b){ return a + b; });
        this.totalVendorSales = this.formatCurrency(totalSales);
        this.cd.detectChanges();
      }
    });
  }

  showInactiveVendors() {
    this.getData();
  }

  formatDate(orderDate: moment.Moment) {
    return orderDate ? moment(orderDate).format('MM/DD/YYYY h:mm A') : '';
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  onDateChange() {
    this.searchCtrl.setValue('');
    this.toDateCtrl.setValue('');
  }

  clearDateSearch() {
    this.searchCtrl.setValue('');
    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    this.getPaginatedPurchaseOrdersList();
  }
}

export interface LineItem {
  customerName: string;
  pNum: string,
  description: string,
  vendorCode: string,
  vendorPartNumber: string;
  partsLink: string;
  zone: string;
  poDate: string;
  poNumber:string;
  deliveryType: string;
  orderNumber: string;
  pfwBNumber: string;
  deliveryDate: string;
  deliveryRoute: string;
}

