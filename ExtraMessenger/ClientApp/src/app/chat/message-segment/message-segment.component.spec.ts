import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageSegmentComponent } from './message-segment.component';

describe('MessageSegmentComponent', () => {
  let component: MessageSegmentComponent;
  let fixture: ComponentFixture<MessageSegmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MessageSegmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageSegmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
