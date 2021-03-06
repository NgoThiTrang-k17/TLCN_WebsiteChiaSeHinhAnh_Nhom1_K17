import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LayoutComponent } from './layout.component';
import { HomeComponent } from './home.component';
import { AddEditPostComponent } from './add-edit-post/add-edit-post.component';
import { DetailPostComponent } from './detail-post/detail-post.component';
import { DetailsComponent } from '../home/profile/details.component';
import { SearchResultComponent } from './search/search-result/search-result.component';

const profileModule = () => import('./profile/profile.module').then(x => x.ProfileModule);
const accountModule = () => import('../account/account.module').then(x => x.AccountModule);

const routes: Routes = [

  {
    path: '', component: LayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'add-post', component: AddEditPostComponent},
      { path: 'detail-post/:id/:ownerId', component: DetailPostComponent},
      { path: 'detail-post/:id/:ownerId/:commentId', component: DetailPostComponent},
      { path: 'detail-post/:id/:ownerId/:commentId/:onCmt', component: DetailPostComponent},
      { path: 'edit/:id', component: AddEditPostComponent},
      { path: 'account', loadChildren: accountModule,},
      { path: 'detail/:id', loadChildren: profileModule},
      { path: 'detail/:id', component: DetailsComponent},
      { path: 'search/:query', component: SearchResultComponent},
    ]
  },

];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserRoutingModule { }
