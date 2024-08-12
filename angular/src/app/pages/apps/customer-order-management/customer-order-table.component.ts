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
import { Lookup } from 'src/services/interfaces/lookup.model';
import { AlertService } from 'src/services/alert.service';
import { Role } from 'src/services/interfaces/role.model';
import { Customer } from 'src/services/interfaces/customer.model';
import { CreateOrderResult, CustomerDTO, Location, OrderDetail, OrderPaginatedListDTO, TotalSalesDTO } from 'src/services/interfaces/models';
import { Router } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Order, User } from 'src/services/interfaces/models';
import { OrderService } from 'src/services/order.service';
import moment from 'moment';
import { CustomerOrderCreateUpdateComponent } from './customer-order-create-update/customer-order-create-update.component';
import { CustomerService } from 'src/services/customer.service';

@UntilDestroy()
@Component({
  selector: 'vex-customer-order-table',
  templateUrl: './customer-order-table.component.html',
  styleUrls: ['./customer-order-table.component.scss'],
  // standalone: true,
  // imports: [CommonModule, HttpClientModule],
  
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
  ],
})

export class CustomerOrderTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input()
  orderColumns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Invoice #', property: 'invoiceNumber', type: 'text', visible: true },
    { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Customer', property: 'customerName', type: 'text', visible: true },
    { label: 'Account #', property: 'accountNumber', type: 'text', visible: true },
    { label: 'Phone #', property: 'phoneNumber', type: 'text', visible: true },
    { label: 'Shipping To', property: 'shipAddressName', type: 'text', visible: true },
    { label: 'Address', property: 'shipAddress', type: 'text', visible: true },
    { label: 'City', property: 'shipCity', type: 'text', visible: true },
    { label: 'State', property: 'shipState', type: 'text', visible: true },
    { label: 'ZipCode', property: 'shipZipCode', type: 'text', visible: true },
    { label: 'Zone', property: 'shipZone', type: 'text', visible: true },
    { label: 'Discount', property: 'discount', type: 'text', visible: true },
    { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'Price Level', property: 'priceLevelName', type: 'text', visible: true },
    { label: 'Warehouse', property: 'warehouseName', type: 'text', visible: false },
  ];

  imageDefault = "assets/img/pfw_logo_sm.png";
  layoutCtrl = new UntypedFormControl('fullwidth');

  orderPageSize: number = 10;
  orderPageIndex: number = 0;
  orderDataCount: number = 0;
  orderPageSizeOptions: number[] = [10, 20, 50, 100];
  // orderSortColumn: string = '';
  // orderSortOrder: string = '';
  orderSearch: string = '';

  quotePageSize: number = 10;
  quotePageIndex: number = 0;
  quoteDataCount: number = 0;
  quotePageSizeOptions: number[] = [10, 20, 50, 100];
  // quoteSortColumn: string = '';
  // quoteSortOrder: string = '';
  quoteSearch: string = '';

  orderDataSource: MatTableDataSource<Order> | null;
  quoteDataSource: MatTableDataSource<Order> | null;
  selection = new SelectionModel<Order>(true, []);
  orderSearchCtrl = new UntypedFormControl();
  quoteSearchCtrl = new UntypedFormControl();

  roleList: Role[];
  customerList: Customer[];
  locationList = {} as Location[]
  orderTypeList: Lookup[];
  totalSalesSummaryListCA: TotalSalesDTO[] = [];
  totalSalesSummaryListNV: TotalSalesDTO[] = [];

  totalSalesCA: any = '0';
  totalCreditCA: any = '0';
  totalNetCA: any = '0';
  totalProfitCA: any = '0';
  
  totalSalesNV: any = '0';
  totalCreditNV: any = '0';
  totalNetNV: any = '0';
  totalProfitNV: any = '0';

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  orderId: number;
  data: any;
  currentCustomer: CustomerDTO;
  
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private customerService: CustomerService,
    private orderService: OrderService,
    private alertService: AlertService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerOrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleOrderColumns() {
    return this.orderColumns.filter(column => column.visible).map(column => column.property);
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

    this.getData();
    this.orderDataSource = new MatTableDataSource();
    this.orderSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onOrderFilterChange(value));

    this.quoteDataSource = new MatTableDataSource();
    this.quoteSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onQuoteFilterChange(value));
  }

  ngAfterViewInit() {
    this.orderDataSource.paginator = this.paginator;
    this.orderDataSource.sort = this.sort;
    this.quoteDataSource.paginator = this.paginator;
    this.quoteDataSource.sort = this.sort;
  }

  getData() {
    this.getCurrentCustomer();
    // this.getPaginatedQuotesList();
    this.getPaginatedOrdersList(undefined);
  }

  getCurrentCustomer() {
    this.customerService.getCustomerById(this.currentUser.customerId).subscribe({
      next: (result) => {
        if (result) {
          this.currentCustomer = result;
        }
      },
      error: (e) => console.error(e),
      complete: () => console.info('complete') 
    })
  }

  createOrder() {
    this.dialog.open(CustomerOrderCreateUpdateComponent, {
      disableClose: true,
      height: '100%',
      width: '100%',
      data: { customer: this.currentCustomer, order: undefined }
    }).afterClosed().subscribe((order: Order) => {
      if (order) {
        this.alertService.showBlockUI("Saving Order...");
        this.orderService.createOrder(order).subscribe((result: CreateOrderResult) => {
          if (result.orderResult) {
            this.alertService.successNotification("Order", "Create");
              this.getPaginatedOrdersList(undefined);
              this.alertService.hideBlockUI();
          }
          else {
            this.alertService.failNotification("Order", "Create");
          }
        });
      }
    });
  }

  updateOrder(order: Order) {
    this.dialog.open(CustomerOrderCreateUpdateComponent, {
      disableClose: true,
      height: '100%',
      width: '100%',
      data: { customer: this.currentCustomer, order: order }
    }).afterClosed().subscribe(updateResult => {
      if (updateResult) {
        // Check if Order was Quote
        if (!updateResult.isConvert) {
          this.orderService.updateOrder(updateResult.updatedOrder).subscribe({
            next: (result: boolean) => {
              if (result) {
                this.getPaginatedOrdersList(undefined);
                this.alertService.successNotification("Order", "Update");
                //this.alertService.hideBlockUI();
              }
            },
            error: (e) => { 
              this.alertService.failNotification("Order", "Update");
              console.error(e); 
            },
            complete: () => {
              console.info('complete') 
            }
          });
        }
      }
    })
  }
              

              // updateResult.updatedOrder.isQuote ? this.getPaginatedQuotesList() : this.getPaginatedOrdersList(undefined);

              // this.alertService.successNotification(updateResult.updatedOrder.isQuote ? "Quote" : "Order", "Update");

              // //Check if BackOrer was set
              // if (updateResult.backOrder) {
              //   this.orderService.createOrder(updateResult.backOrder).subscribe((result: CreateOrderResult) => {
              //     if (result.orderResult) {
              //       updateResult.updatedOrder.isQuote ? this.getPaginatedQuotesList() : this.getPaginatedOrdersList(undefined);
              //       this.alertService.successNotification("Back Order", "Create");
              //     }
              //     else {
              //       this.alertService.failNotification("Back Order", "Create");
              //     }
              //   });
              // }
    //         }
    //         else 
    //       });
    //     }
    //     else {
    //       this.alertService.showBlockUI("Saving Order...");
    //       this.orderService.convertQuoteToOrder(updateResult.updatedOrder).subscribe((result: CreateOrderResult) => {
    //         if (result.orderResult) {
    //           this.getPaginatedQuotesList();
    //           this.getPaginatedOrdersList(undefined);
    //           this.alertService.hideBlockUI();
    //         }
    //         else this.alertService.failNotification("Order", "Created");
    //       });
    //     }
    //   }
    //   else {
    //     this.getData();
    //   }
    // });
  // })};

  deleteOrder(user: Order) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (user) {
      // this.userService.deleteOrder([user]).subscribe((result: Order[]) => (this.subject$.next(result)));
    }

    // this.users.splice(this.users.findIndex((existingOrder) => existingOrder.id === user.id), 1);
    // this.selection.deselect(user);
    // this.subject$.next(this.users);
  }

  deleteOrders(users: Order[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (users.length > 0) {
      // this.userService.deleteOrder(users).subscribe((result: Order[]) => (this.subject$.next(result)));
    }

    // users.forEach(c => this.deleteOrder(c));
  }

  printOrder(event: any, row: Order) {
    if (event) {
      event.stopPropagation();
    }
    this.data = this.mapRowTodata(row);
    this.cd.detectChanges();
    setTimeout( () => { 
      window.print(); 
    }, 2000);
  }

  voidOrder(event: any, row: Order) {
    if (event) {
      event.stopPropagation();
    }
    this.alertService.voidNotification("Order " + row.orderNumber).then(answer => {
      if (!answer.isConfirmed) { return; }
      this.orderService.voidOrder(row).subscribe(result => {
        if (result) {
          this.alertService.successNotification('Order', 'Void');
          this.getPaginatedOrdersList(undefined);
        }
        else this.alertService.failNotification('Order', 'Void');
      });
    });
  }

  getOrderVoidable(row: Order) {
    if (row.orderStatusId === 1) {
      for (let od of row.orderDetails) {
        if (od.statusId === null || od.statusId === 3 || od.statusId === 5) {
          return false;
        }
      }
      return true;
    }
    else return false;
  }

   getPaginatedOrdersList(order: Order) {
    if (!this.currentUser.customerId) return;
    this.alertService.showBlockUI('Loading Orders...');
    if (!!this.orderSearch) this.orderSearch = this.orderSearch.trim();
    this.orderService.getCustomerOrdersPaginated(this.currentUser.customerId, this.orderPageSize, this.orderPageIndex, "OrderNumber", "DESC", this.orderSearch).subscribe((result: OrderPaginatedListDTO) => {
      if (result) {
        this.orderDataSource.data = result.data;
        this.orderDataCount = result.recordCount;
        this.alertService.hideBlockUI();
        
        if (order) {
          this.printOrder(undefined, order);
        }

      }
      this.alertService.hideBlockUI();
    });
  }

  getPaginatedQuotesList() {
    this.alertService.showBlockUI('Loading Quotes...');
    if (!!this.quoteSearch) this.quoteSearch = this.quoteSearch.trim();
    this.orderService.getQuotesPaginated(this.quotePageSize, this.quotePageIndex, "QuoteNumber", "DESC", this.quoteSearch).subscribe((result: OrderPaginatedListDTO) => {
      this.quoteDataSource.data = result.data;
      this.quoteDataCount = result.recordCount;
      this.alertService.hideBlockUI();
    });
  }
  
  mapRowTodata(row: Order): any {
    let result =  {
      imagePath: row.billState === 'CA' ? 'assets/img/pfitwest.png' : 'assets/img/partsco.jpg',
      isCreditMemo: row.orderStatusId === 5,
      orderNumber: row.orderNumber,
      quoteNumber: row.quoteNumber,
      isQuote: row.isQuote,
      invoiceNumber: row.invoiceNumber,
      purchaseOrderNumber:row.purchaseOrderNumber,
      address: row.billState === 'CA' ? '3383 OLIVE AVE, SIGNAL HILL CA 90755' : '5151 W Oquendo Rd, Las Vegas, NV 89118',
      printDate: row.orderDate.toLocaleString(),
      phoneNumber: row.billState === 'CA' ? '310-956-4667' : '702-998-8888',
      website: row.billState === 'CA' ? 'PERFECTFITWEST.COM' : 'PartsCoInc.com',
      soldTo: row.customerName,
      soldToAddress: row.billAddress,
      soldToAddress2: row.billCity + ' ' + row.billState + ' ' + row.billZipCode, 
      shipTo: row.shipAddressName,
      shipToAddress: row.shipAddress, 
      shipToAddress2: row.shipCity + ' ' + row.shipState + ' ' + row.shipZipCode, 
      accountNumber: row.accountNumber,
      customerPhoneNumber: row.phoneNumber,
      customerTerms: row.paymentTermName,
      soldBy: row.createdBy,
      notes: row.orderedByNotes,
      orderType: (row.deliveryType === 1) ? 'DELIVERY' : (row.deliveryType === 2) ? 'PICK UP' : 'SHIPPING',
      orderedBy: row.orderedBy,
      orderedByPhone: row.orderedByPhoneNumber,
      zone: row.shipZone,
      vendorCode: '',
      deliveryDate: moment(row.deliveryDate).format('MM/DD/YYYY') + ' - ' + (row.deliveryRoute === 1 ? 'AM' : 'PM'),
      subTotal: Number(row.subTotalAmount).toFixed(2),
      tax: Number(row.totalTax).toFixed(2),
      total: Number(row.totalAmount).toFixed(2),
      staticText1: '20% RESTOCKING FEE AFTER 10 DAYS, NO RETURNS AFTER 30 DAYS',
      staticText2: '50% RESTOCKING FEE FOR NO BAG/BOX ITEMS',
      totalQuantity: row.orderDetails.length,
      lineItems: this.mapRowLineItems(row.orderDetails)
    }

    return result;
  }

  mapRowLineItems(orderDetails: OrderDetail[]): any {
    let result: LineItem[] = [];
    let ordDet = orderDetails.sort((a, b) => a.partNumber.localeCompare(b.partNumber));
    ordDet.forEach(e => {
      const lineItem = {} as LineItem;
      lineItem.quantity = e.orderQuantity;
      lineItem.partNumber =  e.partNumber; 
      lineItem.description = e.yearFrom + '-' + e.yearTo + ' ' + e.partDescription + ', ' + e.mainPartsLinkNumber + ', ' + ((e.onHandQuantity > 0 && e.stocks.filter(e => Number(e.quantity) > 0).length > 0) ? e.stocks.filter(e => Number(e.quantity) > 0)[0].location + ', STOCK' : (e.vendorPartNumber + ', ' + e.vendorCode));
      lineItem.lprice = Number(e.listPrice).toFixed(2);
      lineItem.price = Number(e.wholesalePrice).toFixed(2);
      lineItem.extPrice = Number(e.totalAmount).toFixed(2)
      result.push(lineItem);
    });
    return result;
  }

  onOrderFilterChange(value: string) {
    if (!this.orderDataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.orderSearch = value;
    if (value.length == 0) {
      this.getPaginatedOrdersList(undefined);
    }
  }

  onQuoteFilterChange(value: string) {
    if (!this.quoteDataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.quoteSearch = value;
    if (value.length == 0) {
      this.getPaginatedQuotesList();
    }
  }

  toggleOrderColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  toggleQuoteColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.orderDataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.orderDataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  getRoleName(value: number) {
    if (this.roleList) {
      let entity = this.roleList.find(e => e.id === value);
      return entity ? entity.name : '';
    }
    return '';
  }

  getLocationName(value: number) {
    if (this.locationList) {
      let entity = this.locationList.find(e => e.id === value);
      return entity ? entity.locationName : '';
    }
    return '';
  }

  getBooleanText(value: boolean) {
    return value === false ? 'False' : 'True';
  }

  searchOrders() {
    this.getPaginatedOrdersList(undefined);
  }

  searchQuotes() {
    this.getPaginatedQuotesList();
  }

  onOrderPaginatorClicked(event) {
    this.orderPageIndex = event.pageIndex;
    this.orderPageSize = event.pageSize;
    this.getPaginatedOrdersList(undefined);
  }

  onQuotePaginatorClicked(event) {
    this.quotePageIndex = event.pageIndex;
    this.quotePageSize = event.pageSize;
    this.getPaginatedQuotesList();
  }

  processOrderNumber(row: Order) {
    return row.isQuote ? row.quoteNumber : row.orderNumber;
  }

  processOrderStatus(row: Order) {
    return row.isQuote ? 'Pending' : row.orderStatusName;
  }

  convertIsQuote(IsQuote: boolean) {
    return IsQuote ? 'Quote' : 'Order';
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }

  formatCurrency(amount: number) {
    return (amount) ? Number(amount).toFixed(2) : '0.00';
  }
}

export interface LineItem {
  quantity: number, 
  partNumber: string, 
  description: string, 
  lprice: string, 
  price: string, 
  extPrice: string
}
