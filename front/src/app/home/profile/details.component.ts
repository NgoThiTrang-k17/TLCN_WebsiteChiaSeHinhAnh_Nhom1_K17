﻿import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { PostToCreate, Post, Account } from '@app/_models';
import { AccountService, PostService } from '@app/_services';

@Component({ templateUrl: 'details.component.html' })
export class DetailsComponent {
    account = this.accountService.accountValue;
    public post: PostToCreate;
    public posts: Post[] = [];
    id: number;

    constructor(
        private accountService: AccountService,
        private postService: PostService,
        private route: ActivatedRoute,
        private router: Router,) { }

    ngOnInit() {
        this.id = this.route.snapshot.params['id'];

        this.postService.getAllByUserId(this.id)
            .subscribe(res => {
                this.posts = res as Post[];
                console.log(res);
            });
        
        this.accountService.getById(this.id)
            .subscribe((res:any)=>{
                this.account = res;
            })
        
    }

    public createImgPath = (serverPath: string) => {
        return `http://localhost:5000/${serverPath}`;
    }
}