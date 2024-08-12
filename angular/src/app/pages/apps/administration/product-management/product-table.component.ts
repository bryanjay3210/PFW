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
import { ProductService } from 'src/services/product.service';
import { AlertService } from 'src/services/alert.service';
import { ProductCreateUpdateComponent } from './product-create-update/product-create-update.component';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ProductPaginatedListDTO, Product, User, ProductDTO } from 'src/services/interfaces/models';
import moment from 'moment';

@UntilDestroy()
@Component({
  selector: 'vex-product-table',
  templateUrl: './product-table.component.html',
  styleUrls: ['./product-table.component.scss'],
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
export class ProductTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  columns: TableColumn<Product>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Picture', property: 'imageUrl', type: 'image', visible: true },
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Part Description', property: 'partDescription', type: 'text', visible: true },
    
    { label: 'Parts Link', property: 'partsLinks', type: 'text', visible: true },
    { label: 'OEM', property: 'oeMs', type: 'text', visible: true },
    { label: 'Vendor Codes', property: 'vendorCodes', type: 'text', visible: false },
    { label: 'Vendor Part Numbers', property: 'vendorPartNumbers', type: 'text', visible: true },
    { label: 'Year From', property: 'yearFrom', type: 'text', visible: true },
    { label: 'Year To', property: 'yearTo', type: 'text', visible: true },
    { label: 'Previous Cost', property: 'previousCost', type: 'text', visible: true },
    { label: 'Current Cost', property: 'currentCost', type: 'text', visible: true },
    { label: 'Price Level 1', property: 'priceLevel1', type: 'text', visible: true },

    { label: 'Status', property: 'isActive', type: 'text', visible: true },
    
    { label: 'CA Product', property: 'isCAProduct', type: 'image', visible: true },
    { label: 'NV Product', property: 'isNVProduct', type: 'image', visible: true },

    { label: 'Actions', property: 'actions', type: 'button', visible: true },

    { label: 'Price Level 2', property: 'priceLevel2', type: 'text', visible: false },
    { label: 'Price Level 3', property: 'priceLevel3', type: 'text', visible: false },
    { label: 'Price Level 4', property: 'priceLevel4', type: 'text', visible: false },
    { label: 'Price Level 5', property: 'priceLevel5', type: 'text', visible: false },
    { label: 'Price Level 6', property: 'priceLevel6', type: 'text', visible: false },
    { label: 'Price Level 7', property: 'priceLevel7', type: 'text', visible: false },
    { label: 'Price Level 8', property: 'priceLevel8', type: 'text', visible: false },
    { label: 'OEM List Price', property: 'oemListPrice', type: 'text', visible: false },
    { label: 'Brand', property: 'brand', type: 'text', visible: false },
    { label: 'Category', property: 'categoryId', type: 'text', visible: false },
    { label: 'Sequence', property: 'sequenceId', type: 'text', visible: false },
    { label: 'Status', property: 'statusId', type: 'text', visible: false },
    { label: 'Part Size Id', property: 'partSizeId', type: 'text', visible: false },
    { label: 'Part Size', property: 'partSize', type: 'text', visible: false },
    { label: 'On Receiving Hold', property: 'onReceivingHold', type: 'text', visible: false },
    { label: 'On Order', property: 'onOrder', type: 'text', visible: false },
    { label: 'Availability', property: 'availabilityId', type: 'text', visible: false },
    { label: 'Drop Ship Allowed', property: 'isDropShipAllowed', type: 'text', visible: false },
    { label: 'Website Option', property: 'isWebsiteOption', type: 'text', visible: false },
    { label: 'Date Added', property: 'dateAdded', type: 'text', visible: false }
    
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  imageNotAvailable = "assets/img/imagenotavailable.png";
  
  layoutCtrl = new UntypedFormControl('fullwidth');
  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  dataSource: MatTableDataSource<Product> | null;
  selection = new SelectionModel<Product>(true, []);
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
    private productService: ProductService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ProductManagement);
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
    this.getPaginatedProducts();
  }

  getPaginatedProducts() {
    this.alertService.showBlockUI('Loading Products...');
    if (!!this.search) this.search = this.search.trim();
    this.productService.getProductsPaginated(this.isShowInactive, this.pageSize, this.pageIndex, "PartNumber", "ASC", this.search).subscribe((result: ProductPaginatedListDTO) => {
      this.dataSource.data = result.data;
      this.dataCount = result.recordCount;
      this.alertService.hideBlockUI();
    });
  }

  createProduct() {
    this.dialog.open(ProductCreateUpdateComponent, {
      height: '80%',
      width: '60%',
    }).afterClosed().subscribe((product: Product) => {
      if (product) {
        this.productService.createProduct(product).subscribe((result: Product[]) => {
          if (result) {
            this.getPaginatedProducts();
            this.alertService.successNotification("Product", "Create");
          }
          else {
            this.alertService.failNotification("Product", "Create");
          }
        });
      }
    });
  }

  updateProduct(product: Product) {
    this.dialog.open(ProductCreateUpdateComponent, {
      height: '80%',
      width: '60%',
      data: product
    }).afterClosed().subscribe(updatedProduct => {
      if (updatedProduct) {
        this.productService.updateProduct(updatedProduct).subscribe((result: Product[]) => {
          if (result) {
            this.getPaginatedProducts();
            this.alertService.successNotification("Product", "Update");
          }
          else this.alertService.failNotification("Product", "Update");
        });
      }
    });
  }

  copyProduct(event: any, row: Product) {
    if (event) {
      event.stopPropagation();
    }

    this.alertService.selectCopyType().then(answer => {
      if (!answer.isConfirmed) return;
      
      let partNo = row.partNumber;   
      partNo += answer.value === 'CAPA' ? 'Q' : answer.value;
      
      this.productService.getProductByPartNumber(partNo).subscribe(result => {
        if (result && result.length > 0) {
          let alertSvc = new AlertService();
          alertSvc.copyExistNotification('Part Number: ' + partNo);
          return;
        }
        const copyProduct = JSON.parse(JSON.stringify(row)) as typeof row;
  
          if (copyProduct) {
            copyProduct.partNumber += answer.value === 'CAPA' ? 'Q' : answer.value;
            copyProduct.copyType = answer.value;
            copyProduct.createdBy = this.currentUser.userName;
            copyProduct.createdDate = moment(new Date());
    
            if (copyProduct.partsLinkList && copyProduct.partsLinkList.length > 0) {
              copyProduct.partsLinkList.forEach(e => {
                e.partsLinkNumber += answer.value === 'CAPA' ? 'C' : answer.value === 'NSF' ? 'N' : answer.value;
                e.createdBy = copyProduct.createdBy;
                e.createdDate = copyProduct.createdDate;
              });
            }
    
            this.productService.createProduct(copyProduct).subscribe((result: Product[]) => {
              if (result) {
                this.getPaginatedProducts();
                this.alertService.successNotification("Product", "Copy");
              }
              else {
                this.alertService.failNotification("Product", "Copy");
              }
            });
          }
      });

      
    });

    
  }

  printProduct(event: any, row: Product) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => {
      window.print();
    }, 2000);
  }

  mapRowTodata(e: Product): any {
    let result: LineItem[] = [];
    const lineItem = {} as LineItem;
      lineItem.description = e.partDescription;
      lineItem.pNum =  e.partNumber;
      lineItem.partsLink = e.partsLinks;
      result.push(lineItem);
    return result;
  }


  deleteProduct(event : any, product: Product) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (product) {
      // this.userService.deleteProduct([user]).subscribe((result: Product[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingProduct) => existingProduct.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deleteProducts(inventories: Product[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deleteProduct(inventories).subscribe((result: Product[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deleteProduct(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedProducts();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedProducts();
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
    this.getPaginatedProducts();
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchProducts() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedProducts();
  }

  showInactiveProducts() {
    this.getData();
  }

  setDefaultImage(event: any) {
    event.src = this.imageNotAvailable;
    this.cd.detectChanges();
  }
}

export interface LineItem {
  pNum: string,
  description: string,
  partsLink: string;
}