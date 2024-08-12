import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { CustomerCreateUpdateComponent } from './customer-create-update/customer-create-update.component';
import { SelectionModel } from '@angular/cdk/collections';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MatSelectChange } from '@angular/material/select';
import { CustomerService } from 'src/services/customer.service';
import { Link } from 'src/@vex/interfaces/link.interface';
import { Router } from '@angular/router';
import { scaleIn400ms } from 'src/@vex/animations/scale-in.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { scaleFadeIn400ms } from 'src/@vex/animations/scale-fade-in.animation';
import { LookupService } from 'src/services/lookup.service';
import { AlertService } from 'src/services/alert.service';
import moment from 'moment';
import { LocationService } from 'src/services/location.service';
import { ContactService } from 'src/services/contact.service';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { Contact, Customer, CustomerPaginatedListDTO, Location, PaymentTerm } from 'src/services/interfaces/models';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { MenuService } from 'src/services/menu-service';
import { User } from 'src/services/interfaces/models';

@UntilDestroy()
@Component({
  selector: 'vex-customer-table',
  templateUrl: './customer-table.component.html',
  styleUrls: ['./customer-table.component.scss'],
  animations: [
    scaleIn400ms,
    fadeInRight400ms,
    stagger40ms,
    fadeInUp400ms,
    scaleFadeIn400ms
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
export class CustomerTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  links: Link[] = [
    {
      label: 'All Contacts',
      route: '../all'
    },
    {
      label: 'Frequently Contacted',
      route: '../frequent'
    },
    {
      label: 'Starred',
      route: '../starred'
    }
  ];

  layoutCtrl = new UntypedFormControl('fullwidth');

  /**
   * Simulating a service with HTTP that returns Observables
   * You probably want to remove this and do all requests in a service with HTTP
   */
  // subject$: ReplaySubject<Customer[]> = new ReplaySubject<Customer[]>(1);
  // data$: Observable<Customer[]> = this.subject$.asObservable();
  //customers: Customer[];

  pageSize: number = 10;
  pageIndex: number = 0;
  dataCount: number = 0;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  sortColumn: string = '';
  sortOrder: string = '';
  search: string = '';

  @Input()
  columns: TableColumn<Customer>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Business Name', property: 'customerName', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Account #', property: 'accountNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Contact Name', property: 'contactName', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Address Line 1', property: 'addressLine1', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Address Line 2', property: 'addressLine2', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'City', property: 'city', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'State', property: 'state', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Country', property: 'country', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Zip Code', property: 'zipCode', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Phone Number', property: 'phoneNumber', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Fax Number', property: 'faxNumber', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Email', property: 'email', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Price Level', property: 'priceLevelId', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Payment Term', property: 'paymentTermId', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Credit Limit', property: 'creditLimit', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Tax Rate', property: 'taxRate', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Over Balance', property: 'overBalance', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    //{ label: 'Account On Hold', property: 'isHoldAccount', type: 'checkbox', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Account On Hold', property: 'isHoldAccount', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Discount', property: 'discount', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Sellers Permit', property: 'sellersPermit', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Cross Street', property: 'crossStreet', type: 'text', visible: false, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  dataSource: MatTableDataSource<Customer> | null;
  selection = new SelectionModel<Customer>(true, []);
  searchCtrl = new UntypedFormControl();
  paymentTermList: PaymentTerm[];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private customerService: CustomerService,
    private lookupService: LookupService,
    private locationService: LocationService,
    private contactService: ContactService,
    private alertService: AlertService,
    private menuService: MenuService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.CustomerManagement);
    this.access = modulePermission.accessTypeId;
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

    this.menuService.loadMenu();
    
    this.getData();

    this.dataSource = new MatTableDataSource();

    // this.data$.pipe(
    //   filter<Customer[]>(Boolean)
    // ).subscribe(customers => {
    //   this.customers = customers;
    //   this.dataSource.data = customers;
    // });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getData() {

    this.getPaginatedCustomers();
    // this.customerService.getCustomers().subscribe((result: Customer[]) => (this.subject$.next(result))
    //   , error => {
    //     if (error.status === 401) {
    //       this.alertService.unauthorizedNotification();
    //       this.router.navigate(['/login']);
    //     }
    //   });

    this.lookupService.getPaymentTerms().subscribe((result: PaymentTerm[]) => (this.paymentTermList = result));
  }

  getPaginatedCustomers() {
    this.alertService.showBlockUI('Loading Customers...');
    if(!!this.search) this.search = this.search.trim();
    this.search = this.search.replace('&', "<--->");
    this.customerService.getCustomersPaginated(this.pageSize, this.pageIndex, "CustomerName", "ASC", this.search).subscribe((result: CustomerPaginatedListDTO) => {
      this.dataSource.data = result.data;
      this.dataCount = result.recordCount;
      this.alertService.hideBlockUI();
    });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  createCustomer() {
    this.dialog.open(CustomerCreateUpdateComponent, {
      height: '95%',
      width: '60%',
    }).afterClosed().subscribe((customer: Customer) => {
      if (customer) {
        this.customerService.createCustomer(customer).subscribe((result: Customer) => {
          if (result) {
            this.getPaginatedCustomers();
            this.alertService.successNotification("Customer", "Create");

            //let newCustomer = result.reduce((previous, current) => (previous.id > current.id) ? previous : current);
            //this.createCustomerDefaultLocation(result);
          }
          else this.alertService.failNotification("Customer", "Create");
        });
      }
    });
  }

  createCustomerDefaultLocation(newCustomer: Customer) {
    const location = {} as Location;
    location.customerId = newCustomer.id;
    location.locationTypeId = 3;
    location.locationName = 'Base Location';
    location.locationCode = 'BSL';
    location.addressLine1 = newCustomer.addressLine1;
    location.addressLine2 = newCustomer.addressLine2;
    location.city = newCustomer.city;
    location.state = newCustomer.state;
    location.country = newCustomer.country;
    location.zipCode = newCustomer.zipCode;
    location.phoneNumber = newCustomer.phoneNumber;
    location.faxNumber = newCustomer.faxNumber;
    location.email = newCustomer.email;
    location.notes = 'Default Customer Location';
    location.isActive = true;
    location.isDeleted = false;
    location.createdBy = "demo@user.com";
    location.createdDate = moment(new Date());

    this.locationService.createLocation(location).subscribe((result = {} as Location[]) => {
      if (result) {
        //this.alertService.successNotification("Location", "Create");
        let newLocation = result.reduce((previous, current) => (previous.id > current.id) ? previous : current);
        this.createCustomerDefaultContact(newCustomer, newLocation);
      }
      else this.alertService.failNotification("Location", "Create");
    });
  }

  createCustomerDefaultContact(newCustomer: Customer, newLocation = {} as Location) {
    let setEmail = (
      newCustomer.paymentTermId === 4 || 
      newCustomer.paymentTermId === 10 ||
      newCustomer.paymentTermId === 11 ||
      newCustomer.paymentTermId === 12 || 
      newCustomer.paymentTermId === 19) ? true : false;
    const contact = {} as Contact;
    contact.customerId = newLocation.customerId
    contact.locationId = newLocation.id;
    contact.positionTypeId = 1;
    contact.contactName = newCustomer.contactName;
    contact.phoneNumber = newCustomer.phoneNumber;
    contact.email = newCustomer.email;
    contact.notes = "Default Customer Contact";
    contact.isActive = true;
    contact.isDeleted = false;
    contact.createdBy = "demo@user.com";
    contact.createdDate = moment(new Date());
    contact.isEmailCreditMemo = setEmail;
    contact.isEmailOrder = setEmail;
    contact.isEmailStatement = setEmail;

    this.contactService.createContact(contact).subscribe((result: Contact[]) => {
      if (result) {
        //this.alertService.successNotification("Contact", "Create");
      }
      else this.alertService.failNotification("Contact", "Create");
    });
  }

  updateCustomer(customer: Customer) {
    this.dialog.open(CustomerCreateUpdateComponent, {
      height: '95%',
      width: '60%',
      data: customer
    }).afterClosed().subscribe(updatedCustomer => {
      if (updatedCustomer) {
        this.customerService.updateCustomer(updatedCustomer).subscribe((result: boolean) => {
          if (result) {
            this.getPaginatedCustomers();
            this.alertService.successNotification("Customer", "Update");
          }
          else this.alertService.failNotification("Customer", "Update");
        });
      }
    });
  }

  deleteCustomer(customer: Customer) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    // this.customers.splice(this.customers.findIndex((existingCustomer) => existingCustomer.id === customer.id), 1);
    // this.selection.deselect(customer);
    // this.subject$.next(this.customers);
  }

  deleteCustomers(customers: Customer[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    customers.forEach(c => this.deleteCustomer(c));
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
      this.getPaginatedCustomers();
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

  getPaymentTermName(value: number) {
    let result = '';

    if (this.paymentTermList) {
      let pt = this.paymentTermList.find(pt => pt.id === value);
      result = pt ? pt.termName : '';
    }

    return result;
  }

  onPaginatorClicked(event) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getPaginatedCustomers();
  }

  searchCustomers() {
    this.getPaginatedCustomers();
  }
}