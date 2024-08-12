import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { ProductService } from 'src/services/product.service';
import { ProductListDTO, ProductDTOPaginatedListDTO, ProductFilterDTO, User } from 'src/services/interfaces/models';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@UntilDestroy()
@Component({
  selector: 'vex-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class ProductListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";
  imageNotAvailable = "assets/img/imagenotavailable.png";

  products: ProductListDTO[] = [];
  dataSource: MatTableDataSource<ProductListDTO> | null;
  selection = new SelectionModel<ProductListDTO>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSize: number = 100;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [25, 50, 75, 100, 150, 200];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';
  productFilterDTO = {} as ProductFilterDTO;

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  columns: TableColumn<ProductListDTO>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Picture', property: 'imageUrl', type: 'image', visible: true },
    { label: 'Part #', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Description', property: 'partDescription', type: 'text', visible: true },
    { label: 'Year Fr', property: 'yearFrom', type: 'text', visible: true },
    { label: 'Year To', property: 'yearTo', type: 'text', visible: true },
    { label: 'Status', property: 'isActive', type: 'text', visible: true },
    { label: 'CA Product', property: 'isCAProduct', type: 'image', visible: true },
    { label: 'NV Product', property: 'isNVProduct', type: 'image', visible: true },
    // { label: 'Actions', property: 'actions', type: 'button', visible: false }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any, 
    private dialogRef: MatDialogRef<ProductListComponent>,
    private router: Router,
    private productService: ProductService,
    private alertService: AlertService,
    private cd: ChangeDetectorRef
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement || rp.module.code == ModuleCode.CustomerOrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    this.productFilterDTO.year = 0;
    this.productFilterDTO.categoryIds = [];
    this.productFilterDTO.sequenceIds = [];
    this.productFilterDTO.make = '';
    this.productFilterDTO.model = '';
    this.productFilterDTO.state = 0;

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

  getData() {
    if (this.defaults.partNumber !== undefined) {
      this.getPaginatedProductsListByPartNumber(this.defaults.state, this.defaults.partNumber.trim());
    }
    else {
      this.productFilterDTO.year = this.defaults.year;
      this.productFilterDTO.categoryIds = this.defaults.categoryIds;
      this.productFilterDTO.sequenceIds = this.defaults.sequenceIds;
      this.productFilterDTO.make = this.defaults.make;
      this.productFilterDTO.model = this.defaults.model;
      this.productFilterDTO.state = this.defaults.state;
      this.getPaginatedProductsList();
    }
  }

  getPaginatedProductsListByPartNumber(state: number, partNumber: string) {
    this.alertService.showBlockUI('Searching...');
    this.search = partNumber.trim();
    this.productService.getSearchProductsListByPartNumberPaginated(state, this.pageSize, this.pageIndex, "PartNumber", "ASC", this.search).subscribe((result: ProductDTOPaginatedListDTO) => {
      this.dataSource.data = result.data;
      this.dataCount = result.recordCount;

      if (this.defaults.partNumber !== undefined) {
        this.searchCtrl.setValue(this.defaults.partNumber.trim());
        this.defaults.partNumber = undefined;
      }

      this.alertService.hideBlockUI();
    });
  }

  getPaginatedProductsList() {
    this.alertService.showBlockUI('Searching...');
    if (!!this.search) this.search = this.search.trim();
    this.productService.getSearchProductsListByYearMakeModelPaginated(this.productFilterDTO, this.pageSize, this.pageIndex, "PartNumber", "ASC", this.search).subscribe((result: ProductDTOPaginatedListDTO) => {
      this.dataSource.data = result.data;
      this.dataCount = result.recordCount;
      this.alertService.hideBlockUI();
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    
    this.search = value;
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedProductsListByPartNumber(this.defaults.state, this.search);
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  selectProduct(product: ProductListDTO) {
    if (!product.isActive) {
      this.alertService.selectInactiveEntityNotification('Product', product.partNumber);
      return;
    }

    this.selection.toggle(product);
  }

  selectProducts(event) {
    const button = (event.srcElement.disabled === undefined) ? event.srcElement.parentElement : event.srcElement;
    button.setAttribute('disabled', true);
    setTimeout(function () {
      button.removeAttribute('disabled');
    }, 10000);

    let products: ProductListDTO[] = [];
    this.selection.selected.forEach(product => {
      products.push(product);
    });

    this.dialogRef.close(products);
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getData();
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

  searchProducts() {
    this.getPaginatedProductsListByPartNumber(this.defaults.state, this.search.trim());
  }

  setDefaultImage(event: any) {
    event.src = this.imageNotAvailable;
    this.cd.detectChanges();
  }
}
