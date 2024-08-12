import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { aioTableLabels } from 'src/static-data/aio-table-data';
import { AccountsReceivableCreateUpdateComponent } from './accounts-receivable-create-update/accounts-receivable-create-update.component';
import { SelectionModel } from '@angular/cdk/collections';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MatSelectChange } from '@angular/material/select';
import { AutomobileService } from 'src/services/automobile.service';
import { AccountsReceivable } from 'src/services/interfaces/accountsreceivable.model';


@UntilDestroy()
@Component({
  selector: 'vex-accounts-receivable-table',
  templateUrl: './accounts-receivable-table.component.html',
  styleUrls: ['./accounts-receivable-table.component.scss'],
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
export class AccountsReceivableTableComponent implements OnInit, AfterViewInit {

  layoutCtrl = new UntypedFormControl('boxed');

  /**
   * Simulating a service with HTTP that returns Observables
   * You probably want to remove this and do all requests in a service with HTTP
   */
  subject$: ReplaySubject<AccountsReceivable[]> = new ReplaySubject<AccountsReceivable[]>(1);
  data$: Observable<AccountsReceivable[]> = this.subject$.asObservable();
  automobiles: AccountsReceivable[];

  @Input()
  columns: TableColumn<AccountsReceivable>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Image', property: 'image', type: 'image', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Make', property: 'make', type: 'text', visible: true },
    { label: 'Model', property: 'model', type: 'text', visible: true },
    { label: 'Type', property: 'type', type: 'text', visible: true },
    { label: 'Year', property: 'year', type: 'text', visible: true},
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];
  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<AccountsReceivable> | null;
  selection = new SelectionModel<AccountsReceivable>(true, []);
  searchCtrl = new UntypedFormControl();

  labels = aioTableLabels;
  imageDefault = "assets/img/pfw_logo_sm.png";

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(private dialog: MatDialog, private automobileService: AutomobileService) {
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  /**
   * Example on how to get data and pass it to the table - usually you would want a dedicated service with a HTTP request for this
   * We are simulating this request here.
   */
  getData() {
    this.automobileService.getAutomobiles().subscribe((result: AccountsReceivable[])  => (this.subject$.next(result)));
  }

  ngOnInit() {
    this.getData();
    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<AccountsReceivable[]>(Boolean)
    ).subscribe(automobiles => {
      this.automobiles = automobiles;
      this.dataSource.data = automobiles;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  createAccountsReceivable() {
    this.dialog.open(AccountsReceivableCreateUpdateComponent).afterClosed().subscribe((automobile: AccountsReceivable) => {
      /**
       * AccountsReceivable is the updated automobile (if the user pressed Save - otherwise it's null)
       */
      if (automobile) {
        // this.automobileService.createAccountsReceivable(automobile).subscribe((result: AccountsReceivable[])  => (this.subject$.next(result)));

        // this.automobiles.unshift(new AccountsReceivable(automobile));
        // this.subject$.next(this.automobiles);
      }
    });
  }

  updateAccountsReceivable(automobile: AccountsReceivable) {
    this.dialog.open(AccountsReceivableCreateUpdateComponent, {
      data: automobile
    }).afterClosed().subscribe(updatedAccountsReceivable => {
      /**
       * AccountsReceivable is the updated automobile (if the user pressed Save - otherwise it's null)
       */
      if (updatedAccountsReceivable) {

        // this.automobileService.updateAccountsReceivable(updatedAccountsReceivable).subscribe((result: AccountsReceivable[])  => (this.subject$.next(result)));

        // const index = this.automobiles.findIndex((existingAccountsReceivable) => existingAccountsReceivable.id === updatedAccountsReceivable.id);
        // this.automobiles[index] = new AccountsReceivable(updatedAccountsReceivable);
        // this.subject$.next(this.automobiles);
      }
    });
  }

  deleteAccountsReceivable(automobile: AccountsReceivable) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
     if (automobile) {
      // this.automobileService.deleteAccountsReceivable([automobile]).subscribe((result: AccountsReceivable[]) => (this.subject$.next(result)));
    }

    // this.automobiles.splice(this.automobiles.findIndex((existingAccountsReceivable) => existingAccountsReceivable.id === automobile.id), 1);
    // this.selection.deselect(automobile);
    // this.subject$.next(this.automobiles);
  }

  deleteAccountsReceivables(automobiles: AccountsReceivable[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (automobiles.length > 0) {
      // this.automobileService.deleteAccountsReceivable(automobiles).subscribe((result: AccountsReceivable[]) => (this.subject$.next(result)));
    }

    // automobiles.forEach(c => this.deleteAccountsReceivable(c));
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

  onLabelChange(change: MatSelectChange, row: AccountsReceivable) {
    const index = this.automobiles.findIndex(c => c === row);
    this.subject$.next(this.automobiles);
  }
}
