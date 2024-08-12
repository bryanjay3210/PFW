import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CustomLayoutComponent } from './custom-layout/custom-layout.component';
import { VexRoutes } from '../@vex/interfaces/vex-route.interface';
import { QuicklinkModule, QuicklinkStrategy } from 'ngx-quicklink';
//import { PrintLayoutComponent } from './print-layout/print-layout.component';
//import { InvoiceComponent } from './invoice/invoice.component';

const routes: VexRoutes = [
  //{ path: 'print',
  //  outlet: 'print',
  //  component: PrintLayoutComponent,
  //  children: [
  //    { path: 'invoice', component: InvoiceComponent }
  //  ]
  //},
  {
    path: '',
    redirectTo: 'landing',
    pathMatch: 'full'
  },
  {
    path: 'landing',
    loadChildren: () => import('./pages/pages/landing-page/landing-page.module').then(m => m.LandingPageModule),
  },
  {
    path: 'login',
    loadChildren: () => import('./pages/pages/auth/login/login.module').then(m => m.LoginModule),
  },
  {
    path: 'register',
    loadChildren: () => import('./pages/pages/auth/register/register.module').then(m => m.RegisterModule),
  },
  {
    path: 'forgot-password',
    loadChildren: () => import('./pages/pages/auth/forgot-password/forgot-password.module').then(m => m.ForgotPasswordModule),
  },
  {
    path: 'coming-soon',
    loadChildren: () => import('./pages/pages/coming-soon/coming-soon.module').then(m => m.ComingSoonModule),
  },
  {
    path: '',
    component: CustomLayoutComponent,
    children: [
      // {
      //   path: 'dashboard/analytics',
      //   redirectTo: '/',
      //   pathMatch: 'full'
      // },
      {
        path: 'dashboard/analytics',
        loadChildren: () => import('./pages/dashboards/dashboard-analytics/dashboard-analytics.module').then(m => m.DashboardAnalyticsModule),
      },
      {
        path: 'apps',
        children: [
          {
            path: 'administration/customer-management',
            loadChildren: () => import('./pages/apps/administration/customer-management/customer-table.module').then(m => m.CustomerTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/payment-management',
            loadChildren: () => import('./pages/apps/administration/payment-management/payment-table.module').then(m => m.PaymentTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/product-management',
            loadChildren: () => import('./pages/apps/administration/product-management/product-table.module').then(m => m.ProductTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/user-management',
            loadChildren: () => import('./pages/apps/administration/user-management/user-table.module').then(m => m.UserTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/roles-permissions',
            loadChildren: () => import('./pages/apps/administration/roles-permissions/roles-permissions.module').then(m => m.RolesPermissionsModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/item-masterlist-reference',
            loadChildren: () => import('./pages/apps/administration/item-masterlist-reference/item-masterlist-reference-table.module').then(m => m.ItemMasterlistReferenceTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/vendor-catalog-masterlist',
            loadChildren: () => import('./pages/apps/administration/vendor-catalog-masterlist/vendor-catalog-masterlist-table.module').then(m => m.VendorCatalogMasterlistTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/order-management',
            loadChildren: () => import('./pages/apps/administration/order-management/order-table.module').then(m => m.OrderTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'administration/email-module',
            loadChildren: () => import('./pages/apps/administration/email-module/email-module.module').then(m => m.EmailModuleModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'purchasing-management/vendor-management',
            loadChildren: () => import('./pages/apps/purchasing-management/vendor-management/vendor-table.module').then(m => m.VendorTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'purchasing-management/purchase-order-management',
            loadChildren: () => import('./pages/apps/purchasing-management/purchase-order-management/purchase-order-table.module').then(m => m.PurchaseOrderTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'report-management/statement-report',
            loadChildren: () => import('./pages/apps/report-management/statement-report/statement-report.module').then(m => m.StatementReportModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'report-management/invoice-report',
            loadChildren: () => import('./pages/apps/report-management/invoice-report/invoice-report.module').then(m => m.InvoiceReportModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'report-management/aging-balance-report',
            loadChildren: () => import('./pages/apps/report-management/aging-balance-report/aging-balance-report.module').then(m => m.AgingBalanceReportModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'report-management/customer-sales-report',
            loadChildren: () => import('./pages/apps/report-management/customer-sales-report/customer-sales-report.module').then(m => m.CustomerSalesReportModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'system-maintenance/module-management',
            loadChildren: () => import('./pages/apps/system-maintenance/module-management/module-group-table.module').then(m => m.ModuleGroupTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'warehouse-management/driver-log',
            loadChildren: () => import('./pages/apps/warehouse-management/driver-log/driver-log-table.module').then(m => m.DriverLogTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'warehouse-management/parts-manifest',
            loadChildren: () => import('./pages/apps/warehouse-management/parts-manifest/parts-manifest-table.module').then(m => m.PartsManifestTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'warehouse-management/parts-picking',
            loadChildren: () => import('./pages/apps/warehouse-management/parts-picking/parts-picking-table.module').then(m => m.PartsPickingTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'warehouse-management/put-away',
            loadChildren: () => import('./pages/apps/warehouse-management/put-away/put-away.module').then(m => m.PutAwayModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'warehouse-management/stock-settings',
            loadChildren: () => import('./pages/apps/warehouse-management/stock-settings/stock-settings.module').then(m => m.StockSettingsModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          {
            path: 'external-customer/customer-order-management',
            loadChildren: () => import('./pages/apps/customer-order-management/customer-order-table.module').then(m => m.CustomerOrderTableModule),
            data: {
              toolbarShadowEnabled: true
            }
          },
          // {
          //   path: 'chat',
          //   loadChildren: () => import('./pages/apps/chat/chat.module').then(m => m.ChatModule),
          //   data: {
          //     toolbarShadowEnabled: true
          //   }
          // },
          // {
          //   path: 'mail',
          //   loadChildren: () => import('./pages/apps/mail/mail.module').then(m => m.MailModule),
          //   data: {
          //     toolbarShadowEnabled: true,
          //     scrollDisabled: true
          //   }
          // },
          // {
          //   path: 'social',
          //   loadChildren: () => import('./pages/apps/social/social.module').then(m => m.SocialModule)
          // },
          // {
          //   path: 'contacts',
          //   loadChildren: () => import('./pages/apps/contacts/contacts.module').then(m => m.ContactsModule)
          // },
          // {
          //   path: 'calendar',
          //   loadChildren: () => import('./pages/apps/calendar/calendar.module').then(m => m.CalendarModule),
          //   data: {
          //     toolbarShadowEnabled: true
          //   }
          // },
          // {
          //   path: 'automobiles',
          //   loadChildren: () => import('./pages/apps/automobiles/automobile-table.module').then(m => m.AutomobileTableModule),
          // },
          // {
          //   path: 'aio-table',
          //   loadChildren: () => import('./pages/apps/aio-table/aio-table.module').then(m => m.AioTableModule),
          // },
          // {
          //   path: 'help-center',
          //   loadChildren: () => import('./pages/apps/help-center/help-center.module').then(m => m.HelpCenterModule),
          // },
          // {
          //   path: 'scrumboard',
          //   loadChildren: () => import('./pages/apps/scrumboard/scrumboard.module').then(m => m.ScrumboardModule),
          // },
          // {
          //   path: 'editor',
          //   loadChildren: () => import('./pages/apps/editor/editor.module').then(m => m.EditorModule),
          // },
        ]
      },
      {
        path: 'pages',
        children: [
          {
            path: 'pricing',
            loadChildren: () => import('./pages/pages/pricing/pricing.module').then(m => m.PricingModule)
          },
          {
            path: 'faq',
            loadChildren: () => import('./pages/pages/faq/faq.module').then(m => m.FaqModule)
          },
          {
            path: 'guides',
            loadChildren: () => import('./pages/pages/guides/guides.module').then(m => m.GuidesModule)
          },
          {
            path: 'invoice',
            loadChildren: () => import('./pages/pages/invoice/invoice.module').then(m => m.InvoiceModule)
          },
          {
            path: 'error-404',
            loadChildren: () => import('./pages/pages/errors/error-404/error-404.module').then(m => m.Error404Module)
          },
          {
            path: 'error-500',
            loadChildren: () => import('./pages/pages/errors/error-500/error-500.module').then(m => m.Error500Module)
          }
        ]
      },
      {
        path: 'ui',
        children: [
          {
            path: 'components',
            loadChildren: () => import('./pages/ui/components/components.module').then(m => m.ComponentsModule),
          },
          {
            path: 'forms/form-elements',
            loadChildren: () => import('./pages/ui/forms/form-elements/form-elements.module').then(m => m.FormElementsModule),
            data: {
              containerEnabled: true
            }
          },
          {
            path: 'forms/form-wizard',
            loadChildren: () => import('./pages/ui/forms/form-wizard/form-wizard.module').then(m => m.FormWizardModule),
            data: {
              containerEnabled: true
            }
          },
          {
            path: 'icons',
            loadChildren: () => import('./pages/ui/icons/icons.module').then(m => m.IconsModule)
          },
          {
            path: 'page-layouts',
            loadChildren: () => import('./pages/ui/page-layouts/page-layouts.module').then(m => m.PageLayoutsModule),
          },
        ]
      },
      {
        path: 'documentation',
        loadChildren: () => import('./pages/documentation/documentation.module').then(m => m.DocumentationModule),
      },
      {
        path: '**',
        loadChildren: () => import('./pages/pages/errors/error-404/error-404.module').then(m => m.Error404Module)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload',
    preloadingStrategy: QuicklinkStrategy,
    scrollPositionRestoration: 'enabled',
    relativeLinkResolution: 'corrected',
    anchorScrolling: 'enabled',
    useHash: true
  })],
  exports: [RouterModule, QuicklinkModule]
})
export class AppRoutingModule {
}
