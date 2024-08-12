import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
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
import { VendorCatalogService } from 'src/services/vendorcatalog.service';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { VendorCatalog, User } from 'src/services/interfaces/models';
import { VendorCatalogMasterlistCreateUpdateComponent } from './vendor-catalog-masterlist-create-update/vendor-catalog-masterlist-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-vendor-catalog-masterlist-table',
  templateUrl: './vendor-catalog-masterlist-table.component.html',
  styleUrls: ['./vendor-catalog-masterlist-table.component.scss'],
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

export class VendorCatalogMasterlistTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<VendorCatalog>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Vendor Part Number', property: 'vendorPartNumber', type: 'text', visible: true },
    { label: 'Parts Link Number', property: 'partsLinkNumber', type: 'text', visible: true },
    { label: 'Price', property: 'price', type: 'text', visible: true },
    { label: 'On Hand', property: 'onHand', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  subject$: ReplaySubject<VendorCatalog[]> = new ReplaySubject<VendorCatalog[]>(1);
  data$: Observable<VendorCatalog[]> = this.subject$.asObservable();
  inventories: VendorCatalog[];

  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<VendorCatalog> | null;
  selection = new SelectionModel<VendorCatalog>(true, []);
  searchCtrl = new UntypedFormControl();

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private vendorCatalogService: VendorCatalogService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.VendorCatalogMasterlist);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit() {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.getData();

    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<VendorCatalog[]>(Boolean)
    ).subscribe(inventories => {
      this.inventories = inventories;
      this.dataSource.data = inventories;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.alertService.showBlockUI('Loading Vendor Catalogs...');
    this.vendorCatalogService.getVendorCatalogs().subscribe((result: VendorCatalog[]) => {
      this.subject$.next(result);
      this.alertService.hideBlockUI();
    });
  }

  createVendorCatalog() {
    this.dialog.open(VendorCatalogMasterlistCreateUpdateComponent, {
      // height: '90%',
      // width: '60%',
    }).afterClosed().subscribe((vendorCatalog: VendorCatalog) => {
      if (vendorCatalog) {
        this.vendorCatalogService.createVendorCatalog(vendorCatalog).subscribe((result: VendorCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Vendor Catalog", "Create");
          }
          else {
            this.alertService.failNotification("Vendor Catalog", "Create");
          }
        });
      }
    });
  }

  updateVendorCatalog(vendorCatalog: VendorCatalog) {
    this.dialog.open(VendorCatalogMasterlistCreateUpdateComponent, {
      // height: '90%',
      // width: '60%',
      data: vendorCatalog
    }).afterClosed().subscribe(updatedVendorCatalog => {
      if (updatedVendorCatalog) {
        this.vendorCatalogService.updateVendorCatalog(updatedVendorCatalog).subscribe((result: VendorCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Vendor Catalog", "Update");
          }
          else this.alertService.failNotification("Vendor Catalog", "Update");
        });
      }
    });
  }

  deleteVendorCatalog(vendorCatalog: VendorCatalog) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (vendorCatalog) {
      // this.userService.deleteVendorCatalog([user]).subscribe((result: VendorCatalog[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingVendorCatalog) => existingVendorCatalog.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deleteVendorCatalogs(inventories: VendorCatalog[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deleteVendorCatalog(inventories).subscribe((result: VendorCatalog[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deleteVendorCatalog(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
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
}
