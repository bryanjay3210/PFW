import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../@vex/interfaces/table-column.interface';
import { aioTableLabels } from '../../../../static-data/aio-table-data';
import { AutomobileCreateUpdateComponent } from './automobile-create-update/automobile-create-update.component';
import { SelectionModel } from '@angular/cdk/collections';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { UntypedFormControl } from '@angular/forms';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MatSelectChange } from '@angular/material/select';
import { AutomobileService } from 'src/services/automobile.service';
import { Automobile } from 'src/services/interfaces/automobile.model';


@UntilDestroy()
@Component({
  selector: 'vex-aio-table',
  templateUrl: './automobile-table.component.html',
  styleUrls: ['./automobile-table.component.scss'],
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
export class AutomobileTableComponent implements OnInit, AfterViewInit {

  layoutCtrl = new UntypedFormControl('boxed');

  /**
   * Simulating a service with HTTP that returns Observables
   * You probably want to remove this and do all requests in a service with HTTP
   */
  subject$: ReplaySubject<Automobile[]> = new ReplaySubject<Automobile[]>(1);
  data$: Observable<Automobile[]> = this.subject$.asObservable();
  automobiles: Automobile[];

  @Input()
  columns: TableColumn<Automobile>[] = [
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
  dataSource: MatTableDataSource<Automobile> | null;
  selection = new SelectionModel<Automobile>(true, []);
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
    this.automobileService.getAutomobiles().subscribe((result: Automobile[])  => (this.subject$.next(result)));
  }

  ngOnInit() {
    this.getData();
    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<Automobile[]>(Boolean)
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

  createAutomobile() {
    this.dialog.open(AutomobileCreateUpdateComponent).afterClosed().subscribe((automobile: Automobile) => {
      /**
       * Automobile is the updated automobile (if the user pressed Save - otherwise it's null)
       */
      if (automobile) {
        this.automobileService.createAutomobile(automobile).subscribe((result: Automobile[])  => (this.subject$.next(result)));

        // this.automobiles.unshift(new Automobile(automobile));
        // this.subject$.next(this.automobiles);
      }
    });
  }

  updateAutomobile(automobile: Automobile) {
    this.dialog.open(AutomobileCreateUpdateComponent, {
      data: automobile
    }).afterClosed().subscribe(updatedAutomobile => {
      /**
       * Automobile is the updated automobile (if the user pressed Save - otherwise it's null)
       */
      if (updatedAutomobile) {

        this.automobileService.updateAutomobile(updatedAutomobile).subscribe((result: Automobile[])  => (this.subject$.next(result)));

        // const index = this.automobiles.findIndex((existingAutomobile) => existingAutomobile.id === updatedAutomobile.id);
        // this.automobiles[index] = new Automobile(updatedAutomobile);
        // this.subject$.next(this.automobiles);
      }
    });
  }

  deleteAutomobile(automobile: Automobile) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
     if (automobile) {
      this.automobileService.deleteAutomobile([automobile]).subscribe((result: Automobile[]) => (this.subject$.next(result)));
    }

    // this.automobiles.splice(this.automobiles.findIndex((existingAutomobile) => existingAutomobile.id === automobile.id), 1);
    // this.selection.deselect(automobile);
    // this.subject$.next(this.automobiles);
  }

  deleteAutomobiles(automobiles: Automobile[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    if (automobiles.length > 0) {
      this.automobileService.deleteAutomobile(automobiles).subscribe((result: Automobile[]) => (this.subject$.next(result)));
    }

    // automobiles.forEach(c => this.deleteAutomobile(c));
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

  onLabelChange(change: MatSelectChange, row: Automobile) {
    const index = this.automobiles.findIndex(c => c === row);
    this.subject$.next(this.automobiles);
  }
}
