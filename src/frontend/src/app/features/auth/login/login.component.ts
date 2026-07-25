import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';
import { AuthService } from '../../../core/services/auth.service';

// Componente de login para autenticação do usuário
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    CardModule,
    InputTextModule,
    PasswordModule,
    MessageModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  carregando = false;
  erro = '';

  formulario;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.formulario = this.formBuilder.nonNullable.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  entrar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.carregando = true;
    this.erro = '';

    this.authService.login(this.formulario.getRawValue()).subscribe({
      next: () => this.router.navigate(['/produtos']),
      error: () => {
        this.erro = 'Usuário ou senha inválidos.';
        this.carregando = false;
      }
    });
  }
}