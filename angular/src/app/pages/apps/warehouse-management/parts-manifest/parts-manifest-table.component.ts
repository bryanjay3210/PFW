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
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { User, PartsManifest, PartsManifestPaginatedListDTO } from 'src/services/interfaces/models';
import moment from 'moment';
import { EmailService } from 'src/services/email.service';
import { PartsManifestCreateUpdateComponent } from './parts-manifest-create-update/parts-manifest-create-update.component';
import { PartsManifestService } from 'src/services/partsmanifest.service';

@UntilDestroy()
@Component({
  selector: 'vex-parts-manifest-table',
  templateUrl: './parts-manifest-table.component.html',
  styleUrls: ['./parts-manifest-table.component.scss'],
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

export class PartsManifestTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<PartsManifest>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Parts Manifest Number', property: 'partsManifestNumber', type: 'text', visible: true },
    { label: 'Driver Name', property: 'driverName', type: 'text', visible: true },
    { label: 'Purpose', property: 'purposeName', type: 'text', visible: true },
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true },
    { label: 'Vendor', property: 'vendorName', type: 'text', visible: true },
    { label: 'Parts Manifest Date', property: 'createdDate', type: 'text', visible: true },
    // { label: 'Status', property: 'statusDetail', type: 'text', visible: true },
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

  dataSource: MatTableDataSource<PartsManifest> | null;
  selection = new SelectionModel<PartsManifest>(true, []);
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
    private partsManifestService: PartsManifestService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.PartsManifest);
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

    // let styleElement = document.getElementById('section-print');
    // styleElement.append('@media print { @page { size: A4 landscape; } }')
  }

  getData() {
    if (this.fromDateCtrl.value !== '' && this.toDateCtrl.value !== '') {
      this.getPaginatedPartsManifestsListByDate();
    }
    else {
      this.getPaginatedPartsManifestsList();
    }
  }

  getPaginatedPartsManifestsList() {
    this.alertService.showBlockUI('Loading Parts Manifests...');
    if (!!this.search) this.search = this.search.trim();
    this.partsManifestService.getPartsManifestsPaginated(this.pageSize, this.pageIndex, "PartsManifestNumber", "DESC", this.search).subscribe((result: PartsManifestPaginatedListDTO) => {
      if (result) {
        this.dataSource.data = result.data;
        this.dataCount = result.recordCount;
        this.alertService.hideBlockUI();
      }
    });
  }

  getPaginatedPartsManifestsListByDate() {
    this.alertService.showBlockUI('Loading Parts Manifests...');
    // let frDate = moment(new Date(this.fromDateCtrl.value)).format('MM/DD/YYYY ');
    // let toDate = moment(new Date(this.toDateCtrl.value)).format('MM/DD/YYYY');

    let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
    let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

    this.partsManifestService.getPartsManifestsByDatePaginated(this.pageSize, this.pageIndex, frDate, toDate).subscribe((result: PartsManifestPaginatedListDTO) => {
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
    this.getData();
  }
  
  createPartsManifest() {
    this.dialog.open(PartsManifestCreateUpdateComponent, {
      height: '80%',
      width: '100%',
    }).afterClosed().subscribe((partsManifest: PartsManifest) => {
      if (partsManifest) {
        this.partsManifestService.createPartsManifest(partsManifest).subscribe((result) => {
          if (result) {
            this.getData();
            this.alertService.successNotification("Parts Manifest", "Create");
            //this.emailService.sendPartsManifestEmails(partsManifest).subscribe(result => {});
          }
          else {
            this.alertService.failNotification("Parts Manifest", "Create");
          }
        });
      }
    });
  }

  updatePartsManifest(partsManifest: PartsManifest) {
    this.dialog.open(PartsManifestCreateUpdateComponent, {
      height: '80%',
      width: '100%',
      data: partsManifest
    }).afterClosed().subscribe((updatePartsManifest: PartsManifest) => {
      if (updatePartsManifest) {
        this.partsManifestService.updatePartsManifest(updatePartsManifest).subscribe((result) => {
          if (result) {
            this.alertService.successNotification("Parts Manifest", "Update");
          }
          else {
            this.alertService.failNotification("Parts Manifest", "Update");
          }

          this.getData();
        });
      }
      else this.getData();
    });
  }

  printPartsManifest(event: any, row: PartsManifest) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => {
      window.print();
    }, 2000);
  }

  mapRowTodata(row: PartsManifest): any {
    const result = {} as PartsManifestResult;
    
    result.date = moment(row.createdDate).format('MM/DD/YYYY'); //moment(new Date(row.createdDate)).format('MM/DD/YYYY');
    result.time = moment(row.createdDate).format('h:mm A');
    result.driverName = row.driverName;
    result.partsManifestNumber = row.partsManifestNumber;
    result.stops = 0;
    result.lineItems = [];
    let on = '';
    let st = '';
    row.partsManifestDetails.forEach(e => {
      // let tempON = e.orderNumber.toString();
      // let tempST = e.shipAddressName;
      // const lineItem = {} as LineItem;
      
      // if (on !== tempON) {
      //   on = tempON;
      //   lineItem.orderNumber = on;
      // }
      // else {
      //   lineItem.orderNumber = '';
      // }
      
      // if (st !== tempST) {
      //   st = tempST;
      //   lineItem.shipTo = st;
      //   result.stops += 1;
      // }
      // else {
      //   lineItem.shipTo = '';
      // }
      
      //lineItem.partNumber = e.partNumber;
      //lineItem.quantity =  e.orderQuantity.toString();
      //lineItem.terms = e.paymentTermName;
      //lineItem.totalAmount = this.formatCurrencyBlank(e.orderTotalAmount);
      //result.lineItems.push(lineItem);
    });

    return result;
  }

  formatDateOnly(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY');
  }

  deletePartsManifest(event: any, row: PartsManifest) {
    if (event) {
      event.stopPropagation();
    }
  }

  deletePartsManifests(inventories: PartsManifest[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deletePartsManifest(inventories).subscribe((result: PartsManifest[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deletePartsManifest(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedPartsManifests();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedPartsManifestsList();
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
    this.getData();
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchPartsManifests() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPartsManifestsList();
  }

  searchPartsManifestsByDate() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPartsManifestsListByDate();
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

export interface PartsManifestResult {
    date: string;
    time: string;
    driverName: string;
    partsManifestNumber: string;
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
