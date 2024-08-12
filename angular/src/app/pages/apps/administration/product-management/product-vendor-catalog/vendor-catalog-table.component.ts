import { SelectionModel } from '@angular/cdk/collections';
import { ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AlertService } from 'src/services/alert.service';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ItemMasterlistReference, ProductVendorCatalog, User, VendorCatalog } from 'src/services/interfaces/models';
import { VendorCatalogService } from 'src/services/vendorcatalog.service';
import { ItemMasterlistReferenceService } from 'src/services/itemmasterlistreference.service';
import { VendorCatalogCreateUpdateComponent } from './vendor-catalog-create-update/vendor-catalog-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-vendor-catalog-table',
  templateUrl: './vendor-catalog-table.component.html',
  styleUrls: ['./vendor-catalog-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class VendorCatalogTableComponent implements OnInit {
  @Input() productId: number;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subject$: ReplaySubject<VendorCatalog[]> = new ReplaySubject<VendorCatalog[]>(1);
  data$: Observable<VendorCatalog[]> = this.subject$.asObservable();
  vendorCatalogs: VendorCatalog[];
  partsLinkList: ItemMasterlistReference[];

  dataSource: MatTableDataSource<VendorCatalog> | null;
  selection = new SelectionModel<VendorCatalog>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  columns: TableColumn<VendorCatalog>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Parts Link Number', property: 'partsLinkNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Vendor Code', property: 'vendorCode', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Vendor Part Number', property: 'vendorPartNumber', type: 'text', visible: true },
    { label: 'Price', property: 'price', type: 'text', visible: true },
    { label: 'On Hand', property: 'onHand', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: VendorCatalog,
    //private dialogRef: MatDialogRef<VendorCatalogCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private vendorCatalogService: VendorCatalogService,
    private itemMasterlistReferenceService: ItemMasterlistReferenceService,
    private alertService: AlertService,
    private changeDetector: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ProductManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    this.getData();

    if (this.defaults) {
      this.mode = 'update';

      this.dataSource = new MatTableDataSource();
      this.data$.pipe(
        filter<VendorCatalog[]>(Boolean)
      ).subscribe(vendorCatalogs => {
        this.vendorCatalogs = vendorCatalogs;
        this.dataSource.data = vendorCatalogs;
      });
    } else {
      this.defaults = {} as VendorCatalog;
    }

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getData() {
    this.itemMasterlistReferenceService.getItemMasterlistReferencesByProductId(this.productId).subscribe((result: ItemMasterlistReference[]) => {
      this.partsLinkList = result;
      this.vendorCatalogService.getVendorCatalogsByPartsLinkNumbers(this.partsLinkList.map(e => e.partsLinkNumber)).subscribe((result: VendorCatalog[]) => (this.subject$.next(result)));
    });
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  ngAfterViewInit() {
    if (this.dataSource) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  createVendorCatalog() {
    this.dialog.open(VendorCatalogCreateUpdateComponent, {
      data: {productId: this.productId, partsLinkList: this.partsLinkList}
    }).afterClosed().subscribe((productVendorCatalog: ProductVendorCatalog) => {
      if (productVendorCatalog) {
        this.vendorCatalogService.createVendorCatalogByProduct(productVendorCatalog).subscribe((result: VendorCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Vendor Catalog", "Create");
          }
          else this.alertService.failNotification("Vendor Catalog", "Create");
        });
      }
    });
  }

  updateVendorCatalog(vendorCatalog: VendorCatalog) {
    this.dialog.open(VendorCatalogCreateUpdateComponent, {
      data: {data: vendorCatalog, productId: this.productId, partsLinkList: this.partsLinkList}
    }).afterClosed().subscribe(productVendorCatalog => {
      if (productVendorCatalog) {
        this.vendorCatalogService.updateVendorCatalogByProduct(productVendorCatalog).subscribe((result: VendorCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Vendor Catalog", "Update");
          }
          else this.alertService.failNotification("Vendor Catalog", "Update");
        });
      }
    });
  }

  deleteVendorCatalog(vendorCatalog: any) { }
  deleteVendorCatalogs(vendorCatalog: any) { }

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

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
