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
import { OrderCreateUpdateComponent } from './order-create-update/order-create-update.component';
import { Role } from 'src/services/interfaces/role.model';
import { Customer } from 'src/services/interfaces/customer.model';
import { CreateOrderResult, CustomerEmailDTO, DeliverySummaryDTO, Location, OrderDetail, OrderPaginatedListDTO, PaymentTerm, TotalSalesDTO } from 'src/services/interfaces/models';
import { ActivatedRoute, Router, RouterStateSnapshot } from '@angular/router';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { Order, User } from 'src/services/interfaces/models';
import { OrderService } from 'src/services/order.service';
import moment from 'moment';
import { CustomerService } from 'src/services/customer.service';
import { CustomerEmailDialog } from './customer-email-dialog/customer-email-dialog';
import { EmailService } from 'src/services/email.service';
import { SharedService } from 'src/@vex/layout/toolbar/toolbar-notifications/services/shared.service';
import { OrderStatusUpdateComponent } from './order-status-update/order-status-update.component';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { LookupService } from 'src/services/lookup.service';

@UntilDestroy()
@Component({
  selector: 'vex-order-table',
  templateUrl: './order-table.component.html',
  styleUrls: ['./order-table.component.scss'],
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

export class OrderTableComponent implements OnInit, AfterViewInit {
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
    { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
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
    { label: 'Price Level', property: 'priceLevelName', type: 'text', visible: true },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Warehouse', property: 'warehouseName', type: 'text', visible: false },
  ];

  quoteColumns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Quote Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true },
    { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'text', visible: true },
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
    { label: 'Price Level', property: 'priceLevelName', type: 'text', visible: true },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Warehouse', property: 'warehouseName', type: 'text', visible: false },
  ];

  webColumns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Quote Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true },
    { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'text', visible: true },
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
    { label: 'Price Level', property: 'priceLevelName', type: 'text', visible: true },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Warehouse', property: 'warehouseName', type: 'text', visible: false },
  ];

  rgaColumns: TableColumn<Order>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true },
    { label: 'Order #', property: 'orderNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Invoice #', property: 'invoiceNumber', type: 'text', visible: true },
    { label: 'Order Date', property: 'orderDate', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Modified By', property: 'modifiedBy', type: 'text', visible: true },
    { label: 'Payment Terms', property: 'paymentTermName', type: 'text', visible: true },
    { label: 'Status', property: 'orderStatusName', type: 'text', visible: true },
    { label: 'Total Amount', property: 'totalAmount', type: 'number', visible: true },
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
    { label: 'Price Level', property: 'priceLevelName', type: 'text', visible: true },
    { label: 'Is Quote', property: 'isQuote', type: 'text', visible: false, cssClasses: ['font-medium'] },
    { label: 'Quote #', property: 'quoteNumber', type: 'text', visible: false, cssClasses: ['font-medium'] },
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

  webPageSize: number = 10;
  webPageIndex: number = 0;
  webDataCount: number = 0;
  webPageSizeOptions: number[] = [10, 20, 50, 100];
  // webSortColumn: string = '';
  // webSortOrder: string = '';
  webSearch: string = '';

  rgaPageSize: number = 10;
  rgaPageIndex: number = 0;
  rgaDataCount: number = 0;
  rgaPageSizeOptions: number[] = [10, 20, 50, 100];
  // rgaSortColumn: string = '';
  // rgaSortOrder: string = '';
  rgaSearch: string = '';

  orderDataSource: MatTableDataSource<Order> | null;
  quoteDataSource: MatTableDataSource<Order> | null;
  webDataSource: MatTableDataSource<Order> | null;
  rgaDataSource: MatTableDataSource<Order> | null;
  selectionOrder = new SelectionModel<Order>(true, []);
  selectionQuote = new SelectionModel<Order>(true, []);
  selectionWeb = new SelectionModel<Order>(true, []);
  selectionRGA = new SelectionModel<Order>(true, []);

  searchTypeCtrl = new UntypedFormControl();
  orderSearchCtrl = new UntypedFormControl();
  quoteSearchCtrl = new UntypedFormControl();
  webSearchCtrl = new UntypedFormControl();
  rgaSearchCtrl = new UntypedFormControl();
  paymentTermCtrl = new UntypedFormControl();
  paymentTermFilterCtrl = new UntypedFormControl();

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
  totalReturnCA: any = '0';

  totalSalesNV: any = '0';
  totalCreditNV: any = '0';
  totalNetNV: any = '0';
  totalProfitNV: any = '0';
  totalReturnNV: any = '0';

  fromDateCtrl = new UntypedFormControl();
  toDateCtrl = new UntypedFormControl();

  paymentTermList: PaymentTerm[] = [];
  paymentTermFilterList = 
  [
    { code:1, name:'Balance Equals 0' },  
    { code:2, name:'Balance Less Than 0' },  
    { code:3, name:'Balance Greater Than 0' },  
  ];

  // returnReasonList = 
  // [
  //   { code:1, name:'Damaged' },
  //   { code:2, name: 'Wrong Part Delivered' }, // -  Required : enter why its wrong
  //   { code:3, name: 'Car Totaled' },
  //   { code:4, name: 'Defective' },
  //   { code:5, name: 'Customer Cancel' },
  //   { code:6, name: 'Price and Billing Adjustment' },
  //   { code:7, name: 'Manager Approval'} // - Required : enter manager/supervisor name
  // ]

  returnReasonList = 
  [
    { code:1, name:'Damaged' },
    { code:2, name: 'Agent Sold Wrong' }, // -  Required : enter why its wrong
    { code:3, name: 'Car Totaled' },
    { code:4, name: 'Defective' },
    { code:5, name: 'Customer Do Not Need' },
    { code:6, name: 'Price and Billing Adjustment' },
    { code:7, name: 'Manager Approval' }, // - Required : enter manager/supervisor name
    { code:8, name: 'Price Too High' },
    { code:9, name: 'Got Somewhere Else' },
    { code:10, name: 'Customer Ordered Wrong' }
  ]

  searchTypeList = 
  [
    {code:0, name:'Select Search Type...'},
    {code:1, name:'Customer Name'},
    {code:2, name:'Phone Number'},
    {code:3, name:'Order Number'},
    {code:4, name:'Purchase Order Number'},
    {code:5, name:'Part Number'},
    {code:6, name:'Main Parts Link Number'},
    {code:7, name:'Main OEM Number'},
    {code:8, name:'Address'},
    {code:9, name:'Created By'},
    {code:10, name:'Invoice Number'},
    {code:11, name:'Account Number'},
    {code:12, name:'Order Status'},
    {code:13, name:'Payment Terms'},
  ]

  selectedTab: number = 0;
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  orderId: number;
  data: any;
  orderNumber: any = undefined;
  isPaymentTermSelected: boolean = false;
  deliverySummary: DeliverySummaryDTO;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private orderService: OrderService,
    private customerService: CustomerService,
    private emailService: EmailService,
    private alertService: AlertService,
    private sharedService: SharedService,
    private lookupService: LookupService,
    private route: ActivatedRoute) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;

    this.router.routeReuseStrategy.shouldReuseRoute = () => {
      return false;
    };

    if (this.router.getCurrentNavigation().extras.state) {
      this.orderNumber = this.router.getCurrentNavigation().extras.state.orderNumber;
      let cm = this.orderService.getOrderByOrderNumber(this.orderNumber).subscribe(result => {
        if (result) {
          this.updateOrder(result);
        }
      });
    }
  }

  get visibleOrderColumns() {
    return this.orderColumns.filter(column => column.visible).map(column => column.property);
  }

  get visibleQuoteColumns() {
    return this.quoteColumns.filter(column => column.visible).map(column => column.property);
  }

  get visibleWebColumns() {
    return this.webColumns.filter(column => column.visible).map(column => column.property);
  }

  get visibleRGAColumns() {
    return this.rgaColumns.filter(column => column.visible).map(column => column.property);
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

    this.orderDataSource = new MatTableDataSource();
    this.fromDateCtrl.setValue('');
    this.toDateCtrl.setValue('');
    this.searchTypeCtrl.setValue(0);
    this.paymentTermCtrl.setValue(0);
    this.paymentTermFilterCtrl.setValue(1);

    this.getPaymentTerms();
    this.getData();

    this.orderSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onOrderFilterChange(value));

    this.quoteDataSource = new MatTableDataSource();
    this.quoteSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onQuoteFilterChange(value));

    this.webDataSource = new MatTableDataSource();
    this.webSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onWebFilterChange(value));

    this.rgaDataSource = new MatTableDataSource();
    this.rgaSearchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onRGAFilterChange(value));
  }

  ngAfterViewInit() {
    this.orderDataSource.paginator = this.paginator;
    this.orderDataSource.sort = this.sort;
    this.quoteDataSource.paginator = this.paginator;
    this.quoteDataSource.sort = this.sort;

    // const firstParam: string = this.route.snapshot.queryParamMap.get('orderNumber');
    // alert(firstParam);
    //alert('HELLO: ' + this.router.getCurrentNavigation().extras.state.orderNumber);
  }

  getPaymentTerms() {
    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => {
      if (result && result.length > 0) {
        this.paymentTermList = result;
        let def = this.paymentTermList[0].id;
        this.paymentTermCtrl.setValue(def);
        this.cd.detectChanges();
      }
    });

  }

  getData() {
    switch (this.selectedTab)
    {
      case 0: 
        if (this.fromDateCtrl.value !== '' && this.toDateCtrl.value !== '') {
        //if (this.toDateCtrl.value && this.fromDateCtrl.value) {
          this.getPaginatedOrdersByDate();
        }
        else {
          this.getPaginatedOrdersList(undefined);
        }
        break;
      case 1:
        this.getPaginatedQuotesList();
        break;
      case 2:
        this.getPaginatedWebOrdersList();
        break;
      case 3:
        this.getPaginatedRGAList();
        break;
      default:
        this.getPaginatedOrdersList(undefined);
        break;
    }
  }

  createOrder() {
    this.dialog.open(OrderCreateUpdateComponent, {
      disableClose: true,
      height: '100%',
      width: '100%',
    }).afterClosed().subscribe((order: Order) => {
      if (order) {
        this.alertService.showBlockUI(order.isQuote ? "Saving Quote..." : "Saving Order...");
        this.orderService.createOrder(order).subscribe((result: CreateOrderResult) => {
          if (result.orderResult) {
            if (order.isQuote) {
              this.getPaginatedQuotesList();
              this.alertService.hideBlockUI();
            }
            else {
              this.clearDateSearch();
              // this.getPaginatedOrdersList(undefined);
              this.alertService.hideBlockUI();
            }
          }
          else {
            this.alertService.failNotification(order.isQuote ? "Quote" : "Order", "Create");
          }
        });
      }
      else {
        this.alertService.hideBlockUI();
      }
    });
  }

  updateOrder(order: Order) {
    this.dialog.open(OrderCreateUpdateComponent, {
      disableClose: true,
      height: '100%',
      width: '100%',
      data: order
    }).afterClosed().subscribe(updateResult => {
      if (updateResult) {
        if (updateResult.isInspectedCode) {
          this.alertService.showBlockUI("Saving RGA Inspected Code...");
          this.orderService.updateOrderInspectedCode(updateResult.updatedOrder).subscribe((result: boolean) => {
            if (result) {
              this.alertService.hideBlockUI();
              this.alertService.successNotification("RGA Inspected Code", "Update");
              setTimeout(()=>{
                this.getData();
              },3000); 
            }
            else this.alertService.failNotification("RGA Inspected Code", "Update");
          });
        }
        else {
          // Check if Order was Quote
          if (!updateResult.isConvert) {
            this.alertService.showBlockUI(updateResult.updatedOrder.isQuote ? "Saving Quote..." : "Saving Order...");
            this.orderService.updateOrder(updateResult.updatedOrder).subscribe((result: boolean) => {
              if (result) {
                //this.alertService.hideBlockUI();
                //updateResult.updatedOrder.isQuote ? this.getPaginatedQuotesList() : this.getPaginatedOrdersList(undefined);
                this.alertService.successNotification(updateResult.updatedOrder.isQuote ? "Quote" : "Order", "Update");
                //Check if BackOrder was set
                if (updateResult.backOrder) {
                  this.alertService.showBlockUI("Creating Back Order...");
                  //updateResult.backOrder.warehouseTracking = "Original Order Number " ;
                  this.orderService.createOrder(updateResult.backOrder).subscribe((result: CreateOrderResult) => {
                    if (result.orderResult) {
                      //this.alertService.hideBlockUI();
                      // updateResult.updatedOrder.isQuote ? this.getPaginatedQuotesList() : this.getPaginatedOrdersList(undefined);
                      this.alertService.successNotification("Back Order", "Create");
                      this.getData();
                    }
                    else {
                      this.alertService.hideBlockUI();
                      this.alertService.failNotification("Back Order", "Create");
                    }
                  });
                }
                else this.getData();
              }
              else this.alertService.failNotification("Order", "Update Error");
            });
          }
          else {
            this.alertService.showBlockUI("Saving Order...");
            this.orderService.convertQuoteToOrder(updateResult.updatedOrder).subscribe((result: CreateOrderResult) => {
              if (result.orderResult) {
                this.getData();
                this.alertService.hideBlockUI();
              }
              else this.alertService.failNotification("Order", "Created");

              this.alertService.hideBlockUI();
            });
          }
        }
      }
      else {
        this.getData();
      }
    });
  }

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
    setTimeout(() => {
      window.print();
    }, 2000);
  }

  emailOrder(event: any, row: Order) {
    if (event) {
      event.stopPropagation();
    }
    this.customerService.getCustomerEmailsById(row.customerId).subscribe(result => {
      if (result) // && result.length > 0)
      {
        this.dialog.open(CustomerEmailDialog, {
          data: { contacts: result, customerId: row.customerId }
        }).afterClosed().subscribe((contact: CustomerEmailDTO) => {
          if (contact) {
            let type = row.isQuote ? 'Quote' : 'Order';
            let number = row.isQuote ? row.quoteNumber : row.orderNumber;

            this.alertService.emailOrderConfirmation(type, number, contact.email).then(result => {
              if (result.isConfirmed) {
                this.emailService.sendOrderEmail(row.id, contact.contactName, contact.email).subscribe(result => {
                  if (result) {
                    this.alertService.successEmailNotification();
                  }
                  else {
                    this.alertService.failedEmailNotification();
                  }
                },
                  error => {
                    this.alertService.failedEmailNotification();
                  });
              }
            });
          }
        });
      }
      else {
        this.alertService.notFoundRecordNotification('Customer Email');
      }
    });
  }

  updateStatus(event: any, row: Order) {
    if (event) {
      event.stopPropagation();
    }

    let toStatusName = row.orderStatusId === 1 ? "Posted" : "Open";
    this.alertService.actionNotificationV2('Update', 'Order Status ', row.orderStatusName + ' to ' + toStatusName).then(result => {
      if (!result.isConfirmed) return;

      if (row.orderStatusId === 1) {
        row.orderStatusId = 2;
        row.orderStatusName = "Posted";
      }
      else if (row.orderStatusId === 2) {
        row.orderStatusId = 1;
        row.orderStatusName = "Open";
      }

      this.orderService.updateOrder(row).subscribe((result: boolean) => {
        if (result) {
          this.alertService.successNotification("Order Status", "Update");
        }
        else this.alertService.failNotification("Order Status", "Update");
      });
    }) 
  }

  updateOrderStatus(event: any, row: Order) {
    if (event) {
      event.stopPropagation();
    }

    this.dialog.open(OrderStatusUpdateComponent, {
      data: row
    }).afterClosed().subscribe((updatedOrder: Order) => {
      if (updatedOrder) {
        this.orderService.updateOrderStatus(updatedOrder).subscribe((result: boolean) => {
          if (result) {
            this.alertService.successNotification("Order", "Update");
            setTimeout(()=>{
              this.getData();
            },2000); 
          }
          else this.alertService.failNotification("Order", "Update");
        });
      }
      else {
        this.getData();
      }
    });
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
        this.getData();
      }
      else this.alertService.failNotification('Order', 'Void');
    });
  });
}

deleteRGAOrder(event: any, row: Order) {
  if (event) {
    event.stopPropagation();
  }
  this.alertService.deleteNotification("RGA " + row.orderNumber).then(answer => {
    if (!answer.isConfirmed) { return; }
    this.orderService.deleteRGAOrder(row).subscribe(result => {
      if (result) {
        this.getData();
        this.alertService.successNotification('RGA ', 'Delete');
      }
      else this.alertService.failNotification('RGA ', 'Delete');
    });
  });
}

getOrderVoidable(row: Order) {
  if (row.orderStatusId === 1) {
    for (let od of row.orderDetails) {
      if (od.statusId === null || od.statusId === 3 || od.statusId === 4 || od.statusId === 5 || od.statusId === 8 || od.statusId === 9) {
        return false;
      }
    }
    return true;
  }
  else return false;
}

getRGAOrderVoidable(row: Order) {
  if (row.orderStatusId === 9) {
    for (let od of row.orderDetails) {
      if (od.rgaInspectedCode !== null) {
        return false;
      }
    }
    return true;
  }
  else return false;
}

getPaginatedOrdersList(order: Order) {
  this.alertService.showBlockUI('Loading Orders...');
  let searchType = (this.searchTypeCtrl.value || this.searchTypeCtrl.value != null || this.searchTypeCtrl.value != undefined) ? this.searchTypeCtrl.value : 0;
  let searchValue = this.orderSearch ? this.orderSearch.trim() : '';
  let paymentTerm = this.paymentTermCtrl.value;
  if (searchType == 13) {
    searchValue = this.paymentTermFilterCtrl.value;
  }

  this.orderService.getOrdersPaginated(searchType, this.orderPageSize, this.orderPageIndex, "OrderNumber", "DESC", searchValue, paymentTerm).subscribe((result: OrderPaginatedListDTO) => {
    if (result) {
      this.orderDataSource.data = result.data;
      this.orderDataCount = result.recordCount;
      this.alertService.hideBlockUI();

      if (order) {
        this.printOrder(undefined, order);
      }

      this.getDailyTotalSalesSummary();
      this.getDeliverySummary();
      this.cd.detectChanges();
    }
    else {
      this.alertService.hideBlockUI();
    }
  });
}

getDeliverySummary() {
  let rawDate = new Date().setHours(0, 0, 0, 0);
  let currentDate = new Date(rawDate).toISOString();

  this.orderService.getDeliverySummary(currentDate).subscribe(result => {
    if (result) {
      this.deliverySummary = result;
      this.cd.detectChanges();
    }
  });
}

getDailyTotalSalesSummary() {
  this.totalSalesSummaryListCA = [];
  this.totalSalesSummaryListNV = [];
  this.totalSalesCA = this.formatCurrency(0);
  this.totalCreditCA = this.formatCurrency(0);
  this.totalNetCA = this.formatCurrency(0);
  this.totalProfitCA = this.formatCurrency(0);
  this.totalReturnCA = this.formatCurrency(0);
  this.totalSalesNV = this.formatCurrency(0);
  this.totalCreditNV = this.formatCurrency(0);
  this.totalNetNV = this.formatCurrency(0);
  this.totalProfitNV = this.formatCurrency(0);
  this.totalReturnNV = this.formatCurrency(0);

  let rawDate = new Date().setHours(0, 0, 0, 0);
  let currentDate = new Date(rawDate).toISOString();

  this.orderService.getDailySalesSummary(currentDate).subscribe(result => {
    if (result) {
      if (result.caSummary.length > 0) {
        this.totalSalesSummaryListCA = result.caSummary;
        let totalSales = this.totalSalesSummaryListCA.map(e => e.salesAmount).reduce(function (a, b) { return a + b; });
        let totalCredit = this.totalSalesSummaryListCA.map(e => e.creditAmount).reduce(function (a, b) { return a + b; });
        let totalNet = this.totalSalesSummaryListCA.map(e => e.netSalesAmount).reduce(function (a, b) { return a + b; });
        let totalCost = this.totalSalesSummaryListCA.map(e => e.unitCost).reduce(function (a, b) { return a + b; });
        let totalProfit = result.caTotalProfitMargin; //(totalNet - totalCost) / totalNet * 100;
        let totalReturnCA = result.caTotalReturnPercentage;

        this.totalSalesCA = this.formatCurrency(totalSales);
        this.totalCreditCA = this.formatCurrency(totalCredit);
        this.totalNetCA = this.formatCurrency(totalNet);
        this.totalProfitCA = this.formatCurrency(totalProfit);
        this.totalReturnCA = this.formatCurrency(totalReturnCA);

      }

      if (result.nvSummary.length > 0) {
        this.totalSalesSummaryListNV = result.nvSummary;
        let totalSales = this.totalSalesSummaryListNV.map(e => e.salesAmount).reduce(function (a, b) { return a + b; });
        let totalCredit = this.totalSalesSummaryListNV.map(e => e.creditAmount).reduce(function (a, b) { return a + b; });
        let totalNet = this.totalSalesSummaryListNV.map(e => e.netSalesAmount).reduce(function (a, b) { return a + b; });
        let totalCost = this.totalSalesSummaryListNV.map(e => e.unitCost).reduce(function (a, b) { return a + b; });
        let totalProfit = result.nvTotalProfitMargin; //(totalNet - totalCost) / totalNet * 100;
        let totalReturnNV = result.nvTotalReturnPercentage;

        this.totalSalesNV = this.formatCurrency(totalSales);
        this.totalCreditNV = this.formatCurrency(totalCredit);
        this.totalNetNV = this.formatCurrency(totalNet);
        this.totalProfitNV = this.formatCurrency(totalProfit);
        this.totalReturnNV = this.formatCurrency(totalReturnNV);
      }

      this.cd.detectChanges();
    }
  });
}

getDailyTotalSalesSummaryByDate() {
  this.totalSalesSummaryListCA = [];
  this.totalSalesSummaryListNV = [];
  this.totalSalesCA = this.formatCurrency(0);
  this.totalCreditCA = this.formatCurrency(0);
  this.totalNetCA = this.formatCurrency(0);
  this.totalProfitCA = this.formatCurrency(0);
  this.totalReturnCA = this.formatCurrency(0);
  this.totalSalesNV = this.formatCurrency(0);
  this.totalCreditNV = this.formatCurrency(0);
  this.totalNetNV = this.formatCurrency(0);
  this.totalProfitNV = this.formatCurrency(0);
  this.totalReturnNV = this.formatCurrency(0);

  // let frDate = new Date(this.fromDateCtrl.value).toLocaleDateString();
  // let toDate = new Date(this.toDateCtrl.value).toLocaleDateString();
  let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
  let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

  this.orderService.getDailySalesSummaryByDate(frDate, toDate).subscribe(result => {
    if (result) {
      if (result.caSummary.length > 0) {
        this.totalSalesSummaryListCA = result.caSummary;
        let totalSales = this.totalSalesSummaryListCA.map(e => e.salesAmount).reduce(function (a, b) { return a + b; });
        let totalCredit = this.totalSalesSummaryListCA.map(e => e.creditAmount).reduce(function (a, b) { return a + b; });
        let totalNet = this.totalSalesSummaryListCA.map(e => e.netSalesAmount).reduce(function (a, b) { return a + b; });
        let totalCost = this.totalSalesSummaryListCA.map(e => e.unitCost).reduce(function (a, b) { return a + b; });
        let totalProfit = (totalNet - totalCost) / totalNet * 100;
        let totalReturnCA = result.caTotalReturnPercentage;

        this.totalSalesCA = this.formatCurrency(totalSales);
        this.totalCreditCA = this.formatCurrency(totalCredit);
        this.totalNetCA = this.formatCurrency(totalNet);
        this.totalProfitCA = this.formatCurrency(totalProfit);
        this.totalReturnCA = this.formatCurrency(totalReturnCA);
      }

      if (result.nvSummary.length > 0) {
        this.totalSalesSummaryListNV = result.nvSummary;
        let totalSales = this.totalSalesSummaryListNV.map(e => e.salesAmount).reduce(function (a, b) { return a + b; });
        let totalCredit = this.totalSalesSummaryListNV.map(e => e.creditAmount).reduce(function (a, b) { return a + b; });
        let totalNet = this.totalSalesSummaryListNV.map(e => e.netSalesAmount).reduce(function (a, b) { return a + b; });
        let totalCost = this.totalSalesSummaryListNV.map(e => e.unitCost).reduce(function (a, b) { return a + b; });
        let totalProfit = (totalNet - totalCost) / totalNet * 100;
        let totalReturnNV = result.nvTotalReturnPercentage;

        this.totalSalesNV = this.formatCurrency(totalSales);
        this.totalCreditNV = this.formatCurrency(totalCredit);
        this.totalNetNV = this.formatCurrency(totalNet);
        this.totalProfitNV = this.formatCurrency(totalProfit);
        this.totalReturnNV = this.formatCurrency(totalReturnNV);
      }
      
      this.cd.detectChanges();
    }
  });
}

getPaginatedQuotesList() {
  this.alertService.showBlockUI('Loading Quotes...');
  if (!!this.quoteSearch) this.quoteSearch = this.quoteSearch.trim();
  this.orderService.getQuotesPaginated(this.quotePageSize, this.quotePageIndex, "QuoteNumber", "DESC", this.quoteSearch).subscribe((result: OrderPaginatedListDTO) => {
    if (result) {
      this.quoteDataSource.data = result.data;
      this.quoteDataCount = result.recordCount;
    }
    
    this.alertService.hideBlockUI();
  });
}

getPaginatedWebOrdersList() {
  this.alertService.showBlockUI('Loading Web Orders...');
  if (!!this.webSearch) this.webSearch = this.webSearch.trim();
  this.orderService.getWebOrdersPaginated(this.webPageSize, this.webPageIndex, "QuoteNumber", "DESC", this.webSearch).subscribe((result: OrderPaginatedListDTO) => {
    if (result) {
      this.webDataSource.data = result.data;
      this.webDataCount = result.recordCount;
    }
    
    this.alertService.hideBlockUI();
  });
}

getPaginatedRGAList() {
  this.alertService.showBlockUI('Loading RGA Orders...');
  if (!!this.rgaSearch) this.rgaSearch = this.rgaSearch.trim();
  this.orderService.getRGAOrdersPaginated(this.rgaPageSize, this.rgaPageIndex, "OrderNumber", "DESC", this.rgaSearch).subscribe((result: OrderPaginatedListDTO) => {
    if (result) {
      this.rgaDataSource.data = result.data;
      this.rgaDataCount = result.recordCount;
    }
    
    this.alertService.hideBlockUI();
  });
}

mapRowTodata(row: Order): any {
  let result = {
    imagePath: row.billState === 'CA' ? 'assets/img/pfitwest.png' : 'assets/img/partsco.jpg',
    orderStatusId: row.orderStatusId,
    rgaType: row.rgaType === 1 ? 'INCOMING' : 'OUTGOING',
    rgaReason: row.rgaReason && row.rgaReason > 0 ? this.returnReasonList.find(e => e.code === row.rgaReason).name : '',
    rgaReasonNotes:   row.rgaReasonNotes,
    isCreditMemo: row.orderStatusId === 5,
    orderNumber: row.orderNumber,
    quoteNumber: row.quoteNumber,
    isQuote: row.isQuote,
    invoiceNumber: row.invoiceNumber,
    purchaseOrderNumber: row.purchaseOrderNumber,
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
    lineItem.partNumber = e.partNumber;
    lineItem.description = e.yearFrom + '-' + e.yearTo + ' ' + e.partDescription + ', ' + e.mainPartsLinkNumber + ', ' +
      ((e.onHandQuantity > 0 && e.stocks.filter(e => Number(e.quantity) > 0).length > 0) ? e.stocks.filter(e => Number(e.quantity) > 0)[0].location + ', STOCK' : (e.vendorPartNumber + ', ' + e.vendorCode));
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
    // this.getPaginatedOrdersList(undefined);
    this.getData();
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

onWebFilterChange(value: string) {
  if (!this.webDataSource) {
    return;
  }
  value = value.trim();
  value = value.toLowerCase();
  this.webSearch = value;
  if (value.length == 0) {
    this.getPaginatedWebOrdersList();
  }
}

onRGAFilterChange(value: string) {
  if (!this.rgaDataSource) {
    return;
  }
  value = value.trim();
  value = value.toLowerCase();
  this.rgaSearch = value;
  if (value.length == 0) {
    this.getPaginatedRGAList();
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

toggleWebColumnVisibility(column, event) {
  event.stopPropagation();
  event.stopImmediatePropagation();
  column.visible = !column.visible;
}

toggleRGAColumnVisibility(column, event) {
  event.stopPropagation();
  event.stopImmediatePropagation();
  column.visible = !column.visible;
}

/** Whether the number of selected elements matches the total number of rows. */
isAllSelectedOrder() {
  const numSelected = this.selectionOrder.selected.length;
  const numRows = this.orderDataSource.data.length;
  return numSelected === numRows;
}

isAllSelectedQuote() {
  const numSelected = this.selectionQuote.selected.length;
  const numRows = this.quoteDataSource.data.length;
  return numSelected === numRows;
}
isAllSelectedWeb() {
  const numSelected = this.selectionWeb.selected.length;
  const numRows = this.webDataSource.data.length;
  return numSelected === numRows;
}

isAllSelectedRGA() {
  const numSelected = this.selectionWeb.selected.length;
  const numRows = this.webDataSource.data.length;
  return numSelected === numRows;
}

/** Selects all rows if they are not all selected; otherwise clear selection. */
masterToggleOrder() {
  this.isAllSelectedOrder() ?
    this.selectionOrder.clear() :
    this.orderDataSource.data.forEach(row => this.selectionOrder.select(row));
}

masterToggleQuote() {
  this.isAllSelectedQuote() ?
    this.selectionQuote.clear() :
    this.quoteDataSource.data.forEach(row => this.selectionQuote.select(row));
}

masterToggleWeb() {
  this.isAllSelectedWeb() ?
    this.selectionWeb.clear() :
    this.webDataSource.data.forEach(row => this.selectionWeb.select(row));
}

masterToggleRGA() {
  this.isAllSelectedRGA() ?
    this.selectionRGA.clear() :
    this.rgaDataSource.data.forEach(row => this.selectionRGA.select(row));
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

getCustomerName(value: number) {
  if (this.customerList) {
    let entity = this.customerList.find(e => e.id === value);
    return entity ? entity.customerName : '';
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
  if (this.fromDateCtrl.value) return;
  this.getPaginatedOrdersList(undefined);
}

searchQuotes() {
  this.getPaginatedQuotesList();
}

searchWebOrders() {
  this.getPaginatedWebOrdersList();
}

searchRGAOrders() {
  this.getPaginatedRGAList();
}

onOrderPaginatorClicked(event) {
  this.orderPageIndex = event.pageIndex;
  this.orderPageSize = event.pageSize;
  this.getData();
  // if (this.fromDateCtrl.value !== '' && this.toDateCtrl.value !== '') {
  //   this.getPaginatedOrdersByDate();
  // }
  // else {
  //   this.getPaginatedOrdersList(undefined);
  // }
}

onQuotePaginatorClicked(event) {
  this.quotePageIndex = event.pageIndex;
  this.quotePageSize = event.pageSize;
  this.getPaginatedQuotesList();
}

onWebPaginatorClicked(event) {
  this.webPageIndex = event.pageIndex;
  this.webPageSize = event.pageSize;
  this.getPaginatedWebOrdersList();
}

onRGAPaginatorClicked(event) {
  this.rgaPageIndex = event.pageIndex;
  this.rgaPageSize = event.pageSize;
  this.getPaginatedRGAList();
}

convertIsQuote(IsQuote: boolean) {
  return IsQuote ? 'Quote' : 'Order';
}

formatDate(orderDate: moment.Moment) {
  return moment(orderDate).format('MM/DD/YYYY h:mm A');
}

formatDateOnly(orderDate: moment.Moment) {
  return moment(orderDate).format('MM/DD/YYYY');
}

formatCurrency(amount: number) {
  return (amount) ? Number(amount).toFixed(2) : '0.00';
}

onDateChange() {
  //this.orderSearchCtrl.setValue('');
  this.toDateCtrl.setValue('');
}

clearDateSearch() {
  this.orderSearchCtrl.setValue('');
  this.fromDateCtrl.setValue('');
  this.toDateCtrl.setValue('');
  this.searchTypeCtrl.setValue(0);
  let def = this.paymentTermList[0].id;
  this.paymentTermCtrl.setValue(def);
  this.paymentTermFilterCtrl.setValue(1);
  this.isPaymentTermSelected = false;
  this.getPaginatedOrdersList(undefined);
}

searchOrdersByDate() {
  this.getPaginatedOrdersByDate();
}

getPaginatedOrdersByDate() {
  this.alertService.showBlockUI('Loading Orders...');
  let searchType = (this.searchTypeCtrl.value || this.searchTypeCtrl.value != null || this.searchTypeCtrl.value != undefined) ? this.searchTypeCtrl.value : 0;
  let searchValue = this.orderSearch ? this.orderSearch.trim() : '';
  let paymentTerm = this.paymentTermCtrl.value;
  if (searchType == 13) {
    searchValue = this.paymentTermFilterCtrl.value;
  }

  // let frDate = new Date(this.fromDateCtrl.value).toLocaleDateString();
  // let toDate = new Date(this.toDateCtrl.value).toLocaleDateString();
  let frDate = moment(new Date(this.fromDateCtrl.value)).toISOString();
  let toDate = moment(new Date(this.toDateCtrl.value)).toISOString();

  if (!!this.orderSearch) this.orderSearch = this.orderSearch.trim();
  this.orderService.getOrdersByDatePaginated(searchType, this.orderPageSize, this.orderPageIndex, frDate, toDate, searchValue, paymentTerm).subscribe((result: OrderPaginatedListDTO) => {
  //this.orderService.getOrdersByDatePaginated(this.orderPageSize, this.orderPageIndex, frDate, toDate).subscribe((result: OrderPaginatedListDTO) => {
    if (result) {
      this.orderDataSource.data = result.data;
      this.orderDataCount = result.recordCount;
      this.getDailyTotalSalesSummaryByDate();
      this.getDeliverySummary();
      this.cd.detectChanges();
    }

    this.alertService.hideBlockUI();
  });
}

onTabChanged(event: MatTabChangeEvent) {
  this.selectedTab = event.index;
  this.getData();
}

searchTypeChanged(event: any) {
  this.isPaymentTermSelected = event.value === 13;
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
