import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService, PostService, AlertService, CommentService } from '@app/_services';
import { Post, Comment } from '@app/_models';

@Component({
  selector: 'app-detail-post',
  templateUrl: './detail-post.component.html'
})
export class DetailPostComponent {

  myForm: FormGroup;
  testForm: any;
  id:number;
  public comments: Comment[] = [];

  account = this.accountService.accountValue;
  comment = this.commentService.commentValue;
  post = new Post;
  constructor(
    private postService: PostService,
    private route: ActivatedRoute,
    private router: Router,
    private accountService: AccountService,
    private commentService: CommentService,
    private formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    this.getRoute(this.route.snapshot.params['id']);
    this.getComment(this.route.snapshot.params['id']);
    // this.accountService.getInfoId(this.route.snapshot.params['ownerId'])
    // .subscribe((res:any)=>{
    //   this.account = res;
    // })
    this.myForm = this.formBuilder.group({
      content: ['', Validators.required],
    });
    this.testForm = new FormData();  
    this.testForm.set('postId', this.post.id);
    this.testForm.set('content', this.myForm.get('content').value);
    
            
  }

  getComment(id:any){
    this.commentService.getAllByPostId(id)
    .subscribe((res:any)=>{
      this.comments = res as Comment[];
    })
  }

  getRoute(id:any){
    this.postService.getPostById(id)
    .subscribe((res:any)=>{
      this.post = res;
    })
  }

  submit() {
    if (this.myForm.invalid) {
      return;
    }
    this.testForm.set('postId', this.post.id);
    this.testForm.set('content', this.myForm.get('content').value);
    // this.testForm.append("postId", this.post.id);
    
    console.log(this.myForm.get('content').value);
    console.log(this.post.id);
    console.log(this.testForm);
    this.commentService.create(this.testForm)
        .subscribe(res => {
            console.log(res);
            alert('Comment Successfully.');
            this.commentService.getAllByPostId(this.post.id)
              .subscribe((res:any)=>{
                this.comments = res as Comment[];
              })
            // this.router.navigate(['../'], { relativeTo: this.route });
        }, error => {
            console.log(error);               
        }) 
  }


  public createImgPath = (serverPath: string) => {
    return `http://localhost:5000/${serverPath}`;
  }

}
