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
import { Vendor, User, PartsPicking, PartsPickingPaginatedListDTO } from 'src/services/interfaces/models';
import moment from 'moment';
//import { PartsPickingService } from 'src/services/purchaseorder.service';
import { PartsPickingCreateUpdateComponent } from './parts-picking-create-update/parts-picking-create-update.component';
import { PartsPickingService } from 'src/services/partspicking.service';
//import { VendorCreateUpdateComponent } from './vendor-create-update/vendor-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-parts-picking-table',
  templateUrl: './parts-picking-table.component.html',
  styleUrls: ['./parts-picking-table.component.scss'],
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

export class PartsPickingTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<PartsPicking>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Pick Number', property: 'pickNumber', type: 'text', visible: true },
    { label: 'Parts Picking Date', property: 'partsPickingDate', type: 'text', visible: true },
    { label: 'Status', property: 'pickStatus', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  warehouseList: any = [{'id': '1', 'name': 'CA Warehouse'}, {'id': '2', 'name': 'NV Warehouse'}];
  layoutCtrl = new UntypedFormControl('fullwidth');
  warehouseCtrl = new UntypedFormControl();
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  
  dataSource: MatTableDataSource<PartsPicking> | null;
  selection = new SelectionModel<PartsPicking>(true, []);
  searchCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;
  data: any;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private partsPickingService: PartsPickingService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PartsPicking);
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
  }

  getData() {
    this.getPaginatedPartsPickingsList();
  }

  getPaginatedPartsPickingsList() {
    this.alertService.showBlockUI('Loading Parts Pickings...');
    if (!!this.search) this.search = this.search.trim();
    this.partsPickingService.getPartsPickingsPaginated(this.pageSize, this.pageIndex, "PickNumber", "DESC", this.search).subscribe((result: PartsPickingPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }
    });
  }

  clearWarehouse() {
    this.warehouseCtrl.setValue(0);
    this.cd.detectChanges();
    // this.alertService.clearNotification("Filters").then(answer => {
    //   if (!answer.isConfirmed) return;
    //   // this.searchCtrl.setValue('');
    //   this.warehouseCtrl.setValue(0);
    //   this.cd.detectChanges();
    //   this.alertService.successNotification('Filters', 'Cleare');
    // });
  }

  createPartsPicking() {
    if (!this.warehouseCtrl.value) {
      this.alertService.requiredNotification('Warehouse is Required!');
      return;
    }

    this.dialog.open(PartsPickingCreateUpdateComponent, {
      height: '80%',
      width: '100%',
      data: {warehouse: this.warehouseCtrl.value ? this.warehouseCtrl.value : 0}
    }).afterClosed().subscribe((partsPicking: PartsPicking) => {
      if (partsPicking) {
        this.partsPickingService.createPartsPicking(partsPicking).subscribe((result: boolean) => {
          if (result) {
            this.getPaginatedPartsPickingsList();
            this.alertService.successNotification("Parts Picking", "Create");
          }
          else {
            this.alertService.failNotification("Parts Picking", "Create");
          }
        });
      }
    });
  }

  updatePartsPicking(partsPicking: PartsPicking) {
    this.dialog.open(PartsPickingCreateUpdateComponent, {
      height: '80%',
      width: '100%',
      data: {details: partsPicking}
    }).afterClosed().subscribe((updatePartsPicking: PartsPicking) => {
      if (updatePartsPicking) {
        this.partsPickingService.updatePartsPicking(updatePartsPicking).subscribe((result: boolean) => {
          if (result) {
            this.alertService.successNotification("Parts Picking", "Update");
          }
          else {
            this.alertService.failNotification("Parts Picking", "Update");
          }

          this.getPaginatedPartsPickingsList();
        });
      }
      else this.getPaginatedPartsPickingsList();
    });
  }

  printPartsPicking(event: any, row: PartsPicking) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => {
      window.print();
    }, 2000);
  }

  mapRowTodata(row: PartsPicking): any {
    let result: LineItem[] = [];
    row.partsPickingDetails.forEach(e => {
      const lineItem = {} as LineItem;
      lineItem.customerName = e.customerName;
      lineItem.description = e.partDescription;
      lineItem.pNum =  e.partNumber;
      lineItem.partsLink = e.mainPartsLinkNumber;
      lineItem.pickNumber = row.pickNumber;
      lineItem.zone = e.shipZone;
      lineItem.stockLocation = e.stockLocation;
      lineItem.stockQuantity = e.stockQuantity.toString();
      //lineItem.poDate = this.formatDateOnly(row.purchaseOrderDate);
      lineItem.deliveryType = e.deliveryMethod;
      lineItem.orderNumber = e.orderNumber.toString();
      lineItem.deliveryDate = this.formatDateOnly(e.deliveryDate);
      lineItem.deliveryRoute = e.deliveryRoute === 1 ? 'AM' : 'PM';
      result.push(lineItem);
    });

    return result;
  }

  formatDateOnly(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY');
  }

  deletePartsPicking(event: any, row: PartsPicking) {
    if (event) {
      event.stopPropagation();
    }
  }

  deletePartsPickings(inventories: PartsPicking[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deletePartsPicking(inventories).subscribe((result: PartsPicking[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deletePartsPicking(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedPartsPickings();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedPartsPickingsList();
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
    //this.dataCount = event.length;
    this.getPaginatedPartsPickingsList();
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchPartsPickings() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPartsPickingsList();
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
}

export interface LineItem {
  customerName: string;
  pNum: string,
  description: string,
  stockLocation: string,
  stockQuantity: string;
  partsLink: string;
  zone: string;
  poDate: string;
  poNumber:string;
  deliveryType: string;
  orderNumber: string;
  pickNumber: string;
  deliveryDate: string;
  deliveryRoute: string;
}

