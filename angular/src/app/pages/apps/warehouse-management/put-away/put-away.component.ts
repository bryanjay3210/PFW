import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy } from '@ngneat/until-destroy';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { AlertService } from 'src/services/alert.service';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Payment, Product, User, WarehouseLocation, WarehouseStockDTO } from 'src/services/interfaces/models';
import moment from 'moment';
import { WarehouseLocationService } from 'src/services/warehouselocation.service';
import { ProductService } from 'src/services/product.service';
import { WarehouseStockService } from 'src/services/warehousestock.service';
import { MatDialog } from '@angular/material/dialog';
import { SearchProductListComponent } from './search-product-list/search-product-list.component';

@UntilDestroy()
@Component({
  selector: 'vex-put-away-table',
  templateUrl: './put-away.component.html',
  styleUrls: ['./put-away.component.scss'],
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
export class PutAwayComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('location', { static: false }) location: ElementRef;
  @ViewChild('product', { static: false }) product: ElementRef;
  @ViewChild('destination', { static: false }) destination: ElementRef;

  @Input()
  columns: TableColumn<WarehouseStockDTO>[] = [
    { label: 'Location', property: 'location', type: 'text', visible: true },
    { label: 'PartNumber', property: 'partNumber', type: 'text', visible: true },
    { label: 'Description', property: 'partDescription', type: 'text', visible: true },
    { label: 'Quantity', property: 'quantity', type: 'text', visible: true },
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

  warehouseStockDTOList: WarehouseStockDTO[] = [];
  dataSource: MatTableDataSource<WarehouseStockDTO> | null;

  selection = new SelectionModel<WarehouseStockDTO>(true, []);
  typeCtrl = new UntypedFormControl();
  stateCtrl = new UntypedFormControl();
  locationCtrl = new UntypedFormControl();
  destinationLocationCtrl = new UntypedFormControl();
  productCtrl = new UntypedFormControl();
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  isShowInactive: boolean = true;

  currentWarehouseLocation: WarehouseLocation = undefined;
  destinationWarehouseLocation: WarehouseLocation = undefined;
  typeList: any = [
    {'id': '1', 'name': 'Batch Putaway'}, 
    {'id': '2', 'name': 'Parts Transfer To Bin'},
    {'id': '3', 'name': 'Bin To Bin Transfer'}, 
    {'id': '4', 'name': 'Cycle Count'}
  ];
  stateList: any = [{'id': '1', 'name': 'CA'}, {'id': '2', 'name': 'NV'}];
  selectedType: string = '0';

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private warehouseLocationService: WarehouseLocationService,
    private productService: ProductService,
    private warehouseStockService: WarehouseStockService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.BinLocationPutAway);
    this.access = modulePermission ? modulePermission.accessTypeId : UserPermission.NoAccess;
  }

  transactionTypeSelectionChange(event: any) {
    this.selectedType = event.value;
    this.stateCtrl.setValue(0);
    this.locationCtrl.setValue('');
    this.productCtrl.setValue('');
    this.destinationLocationCtrl.setValue('');
  }

  stateSelectionChange(event: any) {
    this.locationCtrl.setValue('');
    this.productCtrl.setValue('');
    this.destinationLocationCtrl.setValue('');
  }

  searchLocation() {
    if (this.stateCtrl.value === null) {
      this.alertService.validationRequiredNotification('Please select a State prior to adding location.');
      this.locationCtrl.setValue('');
      this.productCtrl.setValue('');
      return;
    }

    this.warehouseLocationService.getWarehouseLocationByLocation(this.stateCtrl.value, this.locationCtrl.value).subscribe(result => {
      if (result) {
        this.currentWarehouseLocation = result;
        this.product.nativeElement.focus();
      }
      else {
        this.alertService.failNotification('Warehouse Location', 'Search');
        this.location.nativeElement.focus();
      }
    });
  }

  searchDestinationLocation() {
    if (this.stateCtrl.value === null) {
      this.alertService.validationRequiredNotification('Please select a State prior to adding location.');
      this.locationCtrl.setValue('');
      this.productCtrl.setValue('');
      this.destinationLocationCtrl.setValue('');
      return;
    }

    if (this.locationCtrl.value === null) {
      this.alertService.validationRequiredNotification('Please add a location prior to adding a destination location.');
      this.productCtrl.setValue('');
      this.destinationLocationCtrl.setValue('');
      return;
    }

    if (this.locationCtrl.value === this.destinationLocationCtrl.value) {
      this.alertService.validationRequiredNotification('The destination location cannot be the same as the source location.');
      this.productCtrl.setValue('');
      this.destinationLocationCtrl.setValue('');
      return;
    }

    this.warehouseLocationService.getWarehouseLocationByLocation(this.stateCtrl.value, this.destinationLocationCtrl.value).subscribe(result => {
      if (result) {
        this.destinationWarehouseLocation = result;
      }
      else {
        this.alertService.failNotification('Warehouse Location', 'Search');
        this.location.nativeElement.focus();
      }
    });
  }
  
  searchLocationWithStocks() {
    if (this.stateCtrl.value === null) {
      this.alertService.validationRequiredNotification('Please select a State prior to adding location.');
      this.locationCtrl.setValue('');
      this.productCtrl.setValue('');
      return;
    }

    this.warehouseStockDTOList = [];

    this.warehouseLocationService.getWarehouseLocationByLocationWithStocks(this.stateCtrl.value, this.locationCtrl.value).subscribe(result => {
      if (result && result.warehouseLocation.id > 0) {
        this.currentWarehouseLocation = result.warehouseLocation;
        if (!result.stocks || result.stocks.length === 0)
        {
          this.alertService.noStockInLocationNotification(result.warehouseLocation.location);
          this.location.nativeElement.focus();
          return;
        }
        else {

          this.AddProductToList(result.stocks);
          this.destination.nativeElement.focus();
        }
      }
      else {
        this.alertService.failNotification('Warehouse Location', 'Search');
        this.location.nativeElement.focus();
      }
    });
  }

  searchProduct() {
    this.productService.getProductByPartNumber(this.productCtrl.value).subscribe(result => {
      if (result && result.length > 0) {
        if (result.length > 1)
        {
          // Show Popup
          this.dialog.open(SearchProductListComponent, {
            height: '60%',
            width: '60%',
            data: result
          }).afterClosed().subscribe((product: Product[]) => {
            if (product && product.length > 0) {
              this.AddProductToList(product);
            }
          });
        }
        else {
          this.AddProductToList(result);
        }
      }
      else {
        this.alertService.failNotification('Product', 'Search');
      }
    });
  }

  searchProductInLocation() {
    this.productService.getProductInLocationByPartNumber(this.currentWarehouseLocation.id, this.productCtrl.value).subscribe(result => {
      if (result && result.length > 0) {
        if (result.length > 1)
        {
          // Show Popup
          this.dialog.open(SearchProductListComponent, {
            height: '60%',
            width: '60%',
            data: result
          }).afterClosed().subscribe((product: Product[]) => {
            if (product && product.length > 0) {
              this.AddProductToList(product);    
            }
          });
        }
        else {
          this.AddProductToList(result);
        }
      }
      else {
        this.alertService.failOnLocationNotification('Product', this.productCtrl.value, this.locationCtrl.value);
      }
    });
  }

  
  private AddProductToList(products: Product[]) {
    if (!this.currentWarehouseLocation) {
      this.alertService.requiredNotification('Please input location prior to adding products.')
      this.productCtrl.setValue('');
      this.location.nativeElement.focus();
      this.cd.detectChanges();
      return;
    }

    products.forEach((product: Product) => {
      let exist = this.warehouseStockDTOList.find(e => e.productId === product.id && e.warehouseLocationId === this.currentWarehouseLocation.id);
      if (exist) {
        exist.quantity += 1;
        if (this.selectedType === '4') {
          exist.quantity = product.stockQuantity;
        }
      }
      else {
        const stock = {} as WarehouseStockDTO;
        stock.location = this.currentWarehouseLocation.location;
        stock.partDescription = product.partDescription;
        stock.partNumber = product.partNumber;
        stock.productId = product.id;
        stock.quantity = 1

        if (product.stockQuantity != null && (this.selectedType === '3' || this.selectedType === '4')) {
          stock.quantity = product.stockQuantity
        }
        
        stock.warehouseId = this.currentWarehouseLocation.warehouseId;
        stock.warehouseLocationId = this.currentWarehouseLocation.id;

        stock.isActive = true;
        stock.isDeleted = false;
        stock.createdBy = this.currentUser.userName;
        stock.createdDate = moment(new Date());
        stock.modifiedBy = this.currentUser.userName;
        stock.modifiedDate = moment(new Date());

        this.warehouseStockDTOList.push(stock);
      }
    });
    

    this.dataSource.data = this.warehouseStockDTOList;
    this.productCtrl.setValue('');
    this.cd.detectChanges();
  }

  updateWarehouseStocks() {
    this.alertService.updateNotification("Warehouse Stocks").then(answer => {
      if (!answer.isConfirmed) return;
      this.warehouseStockService.updateWarehouseStocks(this.warehouseStockDTOList).subscribe(result => {
        if (result) {
          this.stateCtrl.setValue(0);
          this.locationCtrl.setValue('');
          this.productCtrl.setValue('');
          this.warehouseStockDTOList = [];
          this.dataSource.data = this.warehouseStockDTOList;
          this.cd.detectChanges();
          this.alertService.successNotification('Warehouse Stocks', 'Update');
        }
        else this.alertService.failNotification('Warehouse Stocks', 'Update');
      });
    });
  }

  updateCycleCount() {
    this.alertService.updateNotification("Warehouse Stocks").then(answer => {
      if (!answer.isConfirmed) return;

      this.warehouseStockDTOList.forEach(e => {
        if (e.quantity < 0) {
          this.alertService.negativeValueNotification('Part ' + e.partNumber + ' negative stock quantity is not allowed!');
          return;
        }
      });

      this.warehouseStockService.updateCycleCount(this.warehouseStockDTOList).subscribe(result => {
        if (result) {
          this.stateCtrl.setValue(0);
          this.locationCtrl.setValue('');
          this.productCtrl.setValue('');
          this.warehouseStockDTOList = [];
          this.dataSource.data = this.warehouseStockDTOList;
          this.cd.detectChanges();
          this.alertService.successNotification('Warehouse Stocks', 'Update');
        }
        else this.alertService.failNotification('Warehouse Stocks', 'Update');
      });
    });
  }

  transferPartsToBin() {
    if (!this.warehouseStockDTOList || this.warehouseStockDTOList.length === 0) {
      this.alertService.validationRequiredNotification('Please add part prior to parts transfer.');
      return;
    }

    if (!this.destinationLocationCtrl.value) {
      this.alertService.validationRequiredNotification('Please add a destination location prior to parts transfer.');
      return;
    }

    this.alertService.partsTransferNotification(this.locationCtrl.value, this.destinationLocationCtrl.value).then(answer => {
      if (!answer.isConfirmed) return;

      this.warehouseStockDTOList.forEach(e => {
        e.destinationWarehouseLocationId = this.destinationWarehouseLocation.id;
        e.destinationLocation = this.destinationWarehouseLocation.location;
      });

      this.warehouseStockService.transferWarehouseStocks(this.warehouseStockDTOList).subscribe(result => {
        if (result) {
          this.stateCtrl.setValue(0);
          this.locationCtrl.setValue('');
          this.productCtrl.setValue('');
          this.destinationLocationCtrl.setValue('');
          this.warehouseStockDTOList = [];
          this.dataSource.data = this.warehouseStockDTOList;
          this.cd.detectChanges();
          this.alertService.successNotification('Parts Transfer', 'Transfer');
        }
        else this.alertService.failNotification('Parts Transfer', 'Transfer');
      });
    });
  }

  transferBinToBin() {
    if (!this.warehouseStockDTOList || this.warehouseStockDTOList.length === 0) {
      this.alertService.validationRequiredNotification('Please add part prior to parts transfer.');
      return;
    }

    if (!this.destinationLocationCtrl.value) {
      this.alertService.validationRequiredNotification('Please add a destination location prior to parts transfer.');
      return;
    }

    this.alertService.partsTransferNotification(this.locationCtrl.value, this.destinationLocationCtrl.value).then(answer => {
      if (!answer.isConfirmed) return;

      this.warehouseStockDTOList.forEach(e => {
        e.destinationWarehouseLocationId = this.destinationWarehouseLocation.id;
        e.destinationLocation = this.destinationWarehouseLocation.location;
      });

      this.warehouseStockService.transferWarehouseStocks(this.warehouseStockDTOList).subscribe(result => {
        if (result) {
          this.stateCtrl.setValue(0);
          this.locationCtrl.setValue('');
          this.productCtrl.setValue('');
          this.destinationLocationCtrl.setValue('');
          this.warehouseStockDTOList = [];
          this.dataSource.data = this.warehouseStockDTOList;
          this.cd.detectChanges();
          this.alertService.successNotification('Parts Transfer', 'Transfer');
        }
        else this.alertService.failNotification('Parts Transfer', 'Transfer');
      });
    });
  }

  clearWarehouseStocks() {
    this.alertService.clearNotification("Warehouse Stocks").then(answer => {
      if (!answer.isConfirmed) return;
      this.stateCtrl.setValue(0);
      this.locationCtrl.setValue('');
      this.productCtrl.setValue('');
      this.warehouseStockDTOList = [];
      this.dataSource.data = this.warehouseStockDTOList;
      this.cd.detectChanges();
      this.alertService.successNotification('Warehouse Stocks', 'Cleare');
    });
  }

  handleQuantityOnChangeEvent(row: WarehouseStockDTO, event: any) {
    if (Number(event.target.value) < 0) {
      this.alertService.negativeValueNotification('Negative Stock quantity is not allowed!');
      row.quantity = row.quantity;  
      this.cd.detectChanges();
      return;
    }
    row.quantity = Number(event.target.value);
    this.cd.detectChanges();
  }

  deleteStock(row: WarehouseStockDTO) {
    this.alertService.deleteNotification('Warehouse Stock').then(answer => {
      if (!answer.isConfirmed) return;
      this.warehouseStockDTOList.splice(this.warehouseStockDTOList.findIndex(e => e.warehouseLocationId === row.warehouseLocationId && e.productId === row.productId), 1);
      this.dataSource.data = this.warehouseStockDTOList;
      this.cd.detectChanges();
    });
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

    this.dataSource = new MatTableDataSource();
    this.getData();
    // this.searchCtrl.valueChanges.pipe(
    //   untilDestroyed(this)
    // ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  getData() {
    this.getPaginatedPayments();
  }

  getPaginatedPayments() {
    // this.alertService.showBlockUI('Loading Payments...');
    // if (!!this.search) this.search = this.search.trim();
    // this.paymentService.getPaymentsPaginated(this.isShowInactive, this.pageSize, this.pageIndex, "PartNumber", "ASC", this.search).subscribe((result: PaymentPaginatedListDTO) => {
    //   this.dataSource.data = result.data;
    //   this.dataCount = result.recordCount;
    //   this.alertService.hideBlockUI();
    // });
  }

  createPayment() {
    // this.dialog.open(PaymentCreateUpdateComponent, {
    //   height: '100%',
    //   width: '100%',
    // }).afterClosed().subscribe((payment: Payment) => {
    //   if (payment) {
    //     this.paymentService.createPayment(payment).subscribe((result: Payment) => {
    //       if (result) {
    //         this.getPaginatedPayments();
    //         this.alertService.successNotification("Payment", "Create");
    //       }
    //       else {
    //         this.alertService.failNotification("Payment", "Create");
    //       }
    //     });
    //   }
    // });
  }

  updatePayment() {
    // this.dialog.open(PaymentCreateUpdateComponent, {
    //   height: '100%',
    //   width: '100%',
    //   data: payment
    // }).afterClosed().subscribe(updatedPayment => {
    //   if (updatedPayment) {
    //     this.paymentService.updatePayment(updatedPayment).subscribe((result: boolean) => {
    //       if (result) {
    //         this.getPaginatedPayments();
    //         this.alertService.successNotification("Payment", "Update");
    //       }
    //       else this.alertService.failNotification("Payment", "Update");
    //     });
    //   }
    // });
  }

  deletePayment(payment: Payment) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (payment) {
      // this.userService.deletePayment([user]).subscribe((result: Payment[]) => (this.subject$.next(result)));
    }

    // this.inventories.splice(this.inventories.findIndex((existingPayment) => existingPayment.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.inventories);
  }

  deletePayments(inventories: Payment[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (inventories.length > 0) {
      // this.userService.deletePayment(inventories).subscribe((result: Payment[]) => (this.subject$.next(result)));
    }

    // inventories.forEach(c => this.deletePayment(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    //this.dataSource.filter = value;
    this.search = value;
    //this.getPaginatedPayments();
    if (this.search.length === 0) {
      this.pageIndex = 0;
      this.getPaginatedPayments();
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

  trackByProperty<T>(column: TableColumn<T>) {
    return column.property;
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    //this.dataCount = event.length;
    this.getPaginatedPayments();
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
    //this.getPaginatedTenantLocations();
  }

  searchPayments() {
    //this.search = this.searchCtrl.value.trim();
    this.getPaginatedPayments();
  }

  showInactivePayments() {
    this.getData();
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }
}
