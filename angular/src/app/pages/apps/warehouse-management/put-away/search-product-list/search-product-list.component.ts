import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { ProductService } from 'src/services/product.service';
import { ProductDTOPaginatedListDTO, ProductFilterDTO, User } from 'src/services/interfaces/models';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Product } from 'src/services/interfaces/product.model';

@UntilDestroy()
@Component({
  selector: 'vex-search-product-list',
  templateUrl: './search-product-list.component.html',
  styleUrls: ['./search-product-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})

export class SearchProductListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";
  products: Product[] = [];
  dataSource: MatTableDataSource<Product> | null;
  selection = new SelectionModel<Product>(true, []);
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

  columns: TableColumn<Product>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Picture', property: 'imageUrl', type: 'image', visible: true },
    { label: 'Part #', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Description', property: 'partDescription', type: 'text', visible: true },
    // { label: 'Year Fr', property: 'yearFrom', type: 'text', visible: true },
    // { label: 'Year To', property: 'yearTo', type: 'text', visible: true },
    { label: 'Status', property: 'isActive', type: 'text', visible: true },
    { label: 'CA Product', property: 'isCAProduct', type: 'image', visible: true },
    { label: 'NV Product', property: 'isNVProduct', type: 'image', visible: true },

    // { label: 'Actions', property: 'actions', type: 'button', visible: false }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: Product[], 
    private dialogRef: MatDialogRef<SearchProductListComponent>,
    private router: Router,
    private productService: ProductService,
    private alertService: AlertService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.BinLocationPutAway);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    if (this.currentUser === undefined || this.access === UserPermission.NoAccess) {
      this.alertService.unauthorizedNotification();
      this.router.navigate(['/login']);
      return;
    }

    this.dataSource = new MatTableDataSource();

    if (this.defaults) {
      this.dataSource.data =this.defaults;
    }

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
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

  selectProduct(product: Product) {
    let products: Product[] = []; // Temp solution for IPad. Might revert to original multi-select flow.
    this.selection.toggle(product);
    products.push(product);
    this.dialogRef.close(products);
  }

  selectProducts(event) {
    const button = (event.srcElement.disabled === undefined) ? event.srcElement.parentElement : event.srcElement;
    button.setAttribute('disabled', true);
    setTimeout(function () {
      button.removeAttribute('disabled');
    }, 10000);

    let products: Product[] = [];
    this.selection.selected.forEach(product => {
      products.push(product);
    });

    this.dialogRef.close(products);
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
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
}
