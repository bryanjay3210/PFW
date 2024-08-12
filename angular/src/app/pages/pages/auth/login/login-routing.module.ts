import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { LoginComponent } from './login.component';
import { VexRoutes } from '../../../../../@vex/interfaces/vex-route.interface';
import { RegisterComponent } from '../register/register.component';


const routes: VexRoutes = [
  {
    path: '',
    component: LoginComponent
  }
  // ,
  // {
  //   path: 'register',
  //   component: RegisterComponent
  // }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule]
})
export class LoginRoutingModule {
}
