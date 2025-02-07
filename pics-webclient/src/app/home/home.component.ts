import { Component, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { Image } from '../models/image';
import { ImageService } from '../services/image.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  default = "Loading..."
  isLoading = false;
  allLoaded = false;
  perPage = 12;
  pageCount = 0;
  images: Image[] = [];

  constructor(private router: Router, 
              private imageService: ImageService,
              private authService: AuthService) {
    if (this.authService.getCurrentUser() === null) { 
      this.router.navigateByUrl('login', { replaceUrl: true });
    }
    this.loadImages();
  }

  loadImages() {
    if (this.isLoading || this.allLoaded) {
      return;
    }
    this.isLoading = true;
    this.imageService.getImages(this.pageCount * this.perPage, this.perPage)
      .subscribe({
        next: (data) => {
          if (data.length > 0) {
            this.images.push(...data);
            this.isLoading = false;
            this.pageCount += 1;
          } else {
            this.allLoaded = true;
            if (this.images.length === 0) {
              this.default = "No images found!";
            } else {
              this.default = "The End!";
            }
          }
        },
        error: (err) => console.log(err),
        complete: () => {
          console.log('completed:', this.images.length);
        },
      }
    );
  }

  @HostListener('window:scroll', ['$event'])
  onScroll() {
    if (this.isLoading || this.allLoaded) {
      return;
    }
    if (window.innerHeight + window.scrollY >= document.body.offsetHeight - 100) {
      this.loadImages();
    }
  }

  logOut() {
    this.authService.setCurrentUser(null);
    this.router.navigateByUrl('login', { replaceUrl: true });
  }
}
