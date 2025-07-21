  <script>
    import { Button } from "$lib/components/ui/button";
    import { Input } from "$lib/components/ui/input";
    import { Label } from "$lib/components/ui/label";
    import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "$lib/components/ui/card";

    // Form state
    let email = '';
    let password = '';
    let isLoading = false;
    let error = '';
    let success = false;

    // Validation
    let emailError = '';
    let passwordError = '';

    // Validate email
    function validateEmail(email) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return emailRegex.test(email);
    }

    // Handle form submission
    async function handleLogin() {
      // Reset errors
      emailError = '';
      passwordError = '';
      error = '';

      // Validate form
      if (!email) {
        emailError = 'Email is required';
        return;
      }

      if (!validateEmail(email)) {
        emailError = 'Please enter a valid email address';
        return;
      }

      if (!password) {
        passwordError = 'Password is required';
        return;
      }

      if (password.length < 6) {
        passwordError = 'Password must be at least 6 characters';
        return;
      }

      isLoading = true;

      try {
        const response = await fetch('http://localhost:5027/api/Login/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            email: email,
            password: password
          })
        });

        const data = await response.json();

        if (response.ok) {
          // Login successful
          success = true;
          
          // Store the JWT token (you can use localStorage, sessionStorage, or a store)
          localStorage.setItem('authToken', data.token);
          localStorage.setItem('userEmail', data.email);
          localStorage.setItem('username', data.username);
          
          // Redirect or emit event to parent component
          window.location.href = '/dashboard'; // Or use your routing solution
          
        } else {
          // Login failed
          const raw = await response.text(); // helps diagnose HTML or 404 responses
          console.error("Login failed:", raw);
          error = `Login failed with status ${response.status}`;
          return;
          error = data.message || 'Login failed. Please try again.';
        }
      } catch (err) {
        console.error('Login error:', err);
        error = 'Network error. Please check your connection and try again.';
      } finally {
        isLoading = false;
      }
    }

    // Handle Enter key press
    function handleKeyPress(event) {
      if (event.key === 'Enter') {
        handleLogin();
      }
    }
  </script>

  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <Card class="w-full">
        <CardHeader class="space-y-1">
          <CardTitle class="text-2xl font-bold text-center">Sign in</CardTitle>
          <CardDescription class="text-center">
            Enter your email and password to access your account
          </CardDescription>
        </CardHeader>
        <CardContent class="space-y-4">
          {#if error}
            <div class="p-4 mb-4 text-sm text-red-800 rounded-lg bg-red-50 border border-red-200">
              {error}
            </div>
          {/if}

          {#if success}
            <div class="p-4 mb-4 text-sm text-green-800 rounded-lg bg-green-50 border border-green-200">
              Login successful! Redirecting...
            </div>
          {/if}

          <form on:submit|preventDefault={handleLogin} class="space-y-4">
            <!-- Email Field -->
            <div class="space-y-2">
              <Label for="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="Enter your email"
                bind:value={email}
                on:keypress={handleKeyPress}
                disabled={isLoading}
                class="{emailError ? 'border-red-500' : ''}"
                required
              />
              {#if emailError}
                <p class="text-sm text-red-600">{emailError}</p>
              {/if}
            </div>

            <!-- Password Field -->
            <div class="space-y-2">
              <Label for="password">Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="Enter your password"
                bind:value={password}
                on:keypress={handleKeyPress}
                disabled={isLoading}
                class="{passwordError ? 'border-red-500' : ''}"
                required
              />
              {#if passwordError}
                <p class="text-sm text-red-600">{passwordError}</p>
              {/if}
            </div>

            <!-- Submit Button -->
            <Button
              type="submit"
              class="w-full"
              disabled={isLoading}
            >
              {#if isLoading}
                <span class="mr-2">⏳</span>
                Signing in...
              {:else}
                Sign in
              {/if}
            </Button>
          </form>

          <!-- Additional Links -->
          <div class="text-center space-y-2">
            <p class="text-sm text-gray-600">
              <a href="/forgot-password" class="font-medium text-primary hover:underline">
                Forgot your password?
              </a>
            </p>
            <p class="text-sm text-gray-600">
              Don't have an account?
              <a href="/register" class="font-medium text-primary hover:underline">
                Sign up
              </a>
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>

  <style>
    /* Additional custom styles if needed */
  </style>