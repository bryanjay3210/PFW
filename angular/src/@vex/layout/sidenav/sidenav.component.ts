import { Component, Input, OnInit } from '@angular/core';
import { NavigationService } from '../../services/navigation.service';
import { LayoutService } from '../../services/layout.service';
import { ConfigService } from '../../config/config.service';
import { map, startWith, switchMap } from 'rxjs/operators';
import { PopoverService } from '../../components/popover/popover.service';
import { Observable, of } from 'rxjs';
import { UserMenuComponent } from '../../components/user-menu/user-menu.component';
import { MatDialog } from '@angular/material/dialog';
import { SearchModalComponent } from '../../components/search-modal/search-modal.component';
import { NavigationDropdown, NavigationItem, NavigationLink, NavigationSubheading } from 'src/@vex/interfaces/navigation-item.interface';
import { User } from 'src/services/interfaces/models';

@Component({
  selector: 'vex-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {

  @Input() collapsed: boolean;
  collapsedOpen$ = this.layoutService.sidenavCollapsedOpen$;
  title$ = this.configService.config$.pipe(map(config => config.sidenav.title));
  imageUrl$ = this.configService.config$.pipe(map(config => config.sidenav.imageUrl));
  showCollapsePin$ = this.configService.config$.pipe(map(config => config.sidenav.showCollapsePin));
  userVisible$ = this.configService.config$.pipe(map(config => config.sidenav.user.visible));
  searchVisible$ = this.configService.config$.pipe(map(config => config.sidenav.search.visible));

  userMenuOpen$: Observable<boolean> = of(false);

  //NJPR
  items: any; // = this.navigationService.items;
  currentUser = {} as User;

  constructor(private navigationService: NavigationService,
    private layoutService: LayoutService,
    private configService: ConfigService,
    private readonly popoverService: PopoverService,
    private readonly dialog: MatDialog) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
  }

  ngOnInit() {
    this.initMenu();
    this.items = this.navigationService.items;
  }

  initMenu() {
    let navSubHeading: NavigationSubheading | NavigationDropdown;
    let navLink: NavigationLink;
    let navLinks: NavigationLink[] = [];
    let navDropdown: NavigationDropdown;
    let navDropdowns: NavigationDropdown[] = [];
    let navItems: NavigationItem[] = [];

    navLink = {
      type: 'link',
      label: 'Analytics',
      route: '/dashboard/analytics',
      icon: 'mat:insights',
      routerLinkActiveOptions: { exact: true }
    }

    navSubHeading = {
      type: 'subheading',
      label: 'Dashboards',
      children: [navLink]
    }
    navItems.push(navSubHeading);


    let moduleGroupId: number = 0;
    let parentLabel = ''; 
    this.currentUser.role.rolePermissions.forEach(rp => {
      
      if (moduleGroupId === 0) {
        parentLabel =  rp.moduleGroup.name;
        moduleGroupId = rp.moduleGroupId;
      }

      if (moduleGroupId === rp.moduleGroupId) {
        if (rp.accessTypeId === 1) return;
        navLink = {
          type: 'link',
          label: rp.module.name,
          route: '/apps/' + rp.moduleGroup.name.replace(' ', '-').toLowerCase() + '/' + rp.module.name.replace('Bin Location ', '').replace(' &', '').replace(' ', '-').replace(' ', '-').toLowerCase(),
        }

        navLinks.push(navLink);
      }
      else if (moduleGroupId !== rp.moduleGroupId){
        navDropdown = {
          type: 'dropdown',
          label: parentLabel,
          icon: 'mat:settings_applications',
          children: navLinks
        }

        if (navLinks.length > 0) {
          navDropdowns.push(navDropdown);
        }
        
        moduleGroupId = rp.moduleGroupId;
        parentLabel = rp.moduleGroup.name;
        navLinks = [];

        if (rp.accessTypeId === 1) return;

        navLink = {
          type: 'link',
          label: rp.module.name,
          route: '/apps/' + rp.moduleGroup.name.replace(' ', '-').toLowerCase() + '/' + rp.module.name.replace('Bin Location ', '').replace(' &', '').replace(' ', '-').replace(' ', '-').toLowerCase(),
        }

        navLinks.push(navLink);
      }
    });

    navDropdown = {
      type: 'dropdown',
      label: parentLabel,
      icon: 'mat:settings_applications',
      children: navLinks
    }
    if (navLinks.length > 0) {
      navDropdowns.push(navDropdown);
    }

    navSubHeading = {
      type: 'subheading',
      label: 'Menu Tab Module',
      children: navDropdowns
    }
    navItems.push(navSubHeading);

    this.navigationService.items = navItems;
  }

  collapseOpenSidenav() {
    this.layoutService.collapseOpenSidenav();
  }

  collapseCloseSidenav() {
    this.layoutService.collapseCloseSidenav();
  }

  toggleCollapse() {
    this.collapsed ? this.layoutService.expandSidenav() : this.layoutService.collapseSidenav();
  }

  trackByRoute(index: number, item: NavigationLink): string {
    return item.route;
  }

  openProfileMenu(origin: HTMLDivElement): void {
    this.userMenuOpen$ = of(
      this.popoverService.open({
        content: UserMenuComponent,
        origin,
        offsetY: -8,
        width: origin.clientWidth,
        position: [
          {
            originX: 'center',
            originY: 'top',
            overlayX: 'center',
            overlayY: 'bottom'
          }
        ]
      })
    ).pipe(
      switchMap(popoverRef => popoverRef.afterClosed$.pipe(map(() => false))),
      startWith(true),
    );
  }

  openSearch(): void {
    this.dialog.open(SearchModalComponent, {
      panelClass: 'vex-dialog-glossy',
      width: '100%',
      maxWidth: '600px'
    });
  }
}
