<script>
    import { onMount } from 'svelte';
    import { goto } from '$app/navigation';

    let formData = {
        username: '',
        email: '',
        password: ''
    };

    let errors = {};
    let isLoading = false;
    let successMessage = '';

    // Form validation
    function validateForm() {
        errors = {};
        
        if (!formData.username || formData.username.length < 3) {
            errors.username = 'Username must be at least 3 characters long';
        }
        
        if (!formData.email || !/\S+@\S+\.\S+/.test(formData.email)) {
            errors.email = 'Please enter a valid email address';
        }
        
        if (!formData.password || formData.password.length < 6) {
            errors.password = 'Password must be at least 6 characters long';
        }
        
        return Object.keys(errors).length === 0;
    }

    // Handle form submission
    async function handleSubmit() {
        if (!validateForm()) return;

        isLoading = true;
        errors = {};
        successMessage = '';

        try {
            const response = await fetch('http://localhost:5027/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });

            const data = await response.json();

            if (response.ok) {
                // Success! Store the token and redirect
                localStorage.setItem('authToken', data.token);
                localStorage.setItem('userInfo', JSON.stringify({
                    username: data.username,
                    email: data.email
                }));
                
                successMessage = 'Registration successful! Redirecting...';
                
                // Redirect to dashboard or home page after 2 seconds
                setTimeout(() => {
                    goto('/dashboard'); // Change this to your desired route
                }, 2000);
                
            } else {
                // Handle error response
                errors.general = data.message || 'Registration failed. Please try again.';
            }
        } catch (error) {
            errors.general = 'Network error. Please check your connection and try again.';
        } finally {
            isLoading = false;
        }
    }

    // Clear individual field errors when user starts typing
    function clearFieldError(field) {
        if (errors[field]) {
            errors = { ...errors };
            delete errors[field];
        }
    }
</script>

<div class="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 flex items-center justify-center p-4">
    <!-- Card Container -->
    <div class="bg-white rounded-lg border shadow-sm w-full max-w-md">
        <!-- Card Header -->
        <div class="flex flex-col space-y-1.5 p-6 pb-4">
            <h1 class="text-2xl font-semibold leading-none tracking-tight text-center">Create Account</h1>
            <p class="text-sm text-muted-foreground text-center">Join us today and get started!</p>
        </div>

        <!-- Card Content -->
        <div class="p-6 pt-0 space-y-4">
            <!-- Success Alert -->
            {#if successMessage}
                <div class="relative w-full rounded-lg border border-green-200 bg-green-50 p-4">
                    <div class="flex items-center gap-2">
                        <svg class="h-4 w-4 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <div class="text-sm text-green-800">{successMessage}</div>
                    </div>
                </div>
            {/if}

            <!-- Error Alert -->
            {#if errors.general}
                <div class="relative w-full rounded-lg border border-red-200 bg-red-50 p-4">
                    <div class="flex items-center gap-2">
                        <svg class="h-4 w-4 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16c-.77.833.192 2.5 1.732 2.5z" />
                        </svg>
                        <div class="text-sm text-red-800">{errors.general}</div>
                    </div>
                </div>
            {/if}

            <!-- Form Fields -->
            <div class="space-y-4">
                <!-- Username Field -->
                <div class="space-y-2">
                    <label for="username" class="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                        Username
                    </label>
                    <input
                        id="username"
                        type="text"
                        bind:value={formData.username}
                        on:input={() => clearFieldError('username')}
                        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 {errors.username ? 'border-red-500 focus-visible:ring-red-500' : ''}"
                        placeholder="Enter your username"
                        disabled={isLoading}
                    />
                    {#if errors.username}
                        <p class="text-sm text-red-600">{errors.username}</p>
                    {/if}
                </div>

                <!-- Email Field -->
                <div class="space-y-2">
                    <label for="email" class="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                        Email Address
                    </label>
                    <input
                        id="email"
                        type="email"
                        bind:value={formData.email}
                        on:input={() => clearFieldError('email')}
                        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 {errors.email ? 'border-red-500 focus-visible:ring-red-500' : ''}"
                        placeholder="Enter your email"
                        disabled={isLoading}
                    />
                    {#if errors.email}
                        <p class="text-sm text-red-600">{errors.email}</p>
                    {/if}
                </div>

                <!-- Password Field -->
                <div class="space-y-2">
                    <label for="password" class="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                        Password
                    </label>
                    <input
                        id="password"
                        type="password"
                        bind:value={formData.password}
                        on:input={() => clearFieldError('password')}
                        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 {errors.password ? 'border-red-500 focus-visible:ring-red-500' : ''}"
                        placeholder="Enter your password"
                        disabled={isLoading}
                    />
                    {#if errors.password}
                        <p class="text-sm text-red-600">{errors.password}</p>
                    {/if}
                </div>

                <!-- Submit Button -->
                <button
                    type="submit"
                    on:click={handleSubmit}
                    disabled={isLoading}
                    class="inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 h-10 px-4 py-2 w-full"
                    style="background-color: hsl(var(--primary)); color: hsl(var(--primary-foreground));"
                >
                    {#if isLoading}
                        <svg class="animate-spin -ml-1 mr-3 h-4 w-4" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        Creating Account...
                    {:else}
                        Create Account
                    {/if}
                </button>
            </div>

            <!-- Login Link -->
            <div class="text-center">
                <p class="text-sm text-muted-foreground">
                    Already have an account? 
                    <a href="/login" class="text-primary hover:underline font-medium">
                        Sign in here
                    </a>
                </p>
            </div>
        </div>
    </div>
</div>

<style>
    /* shadcn/ui CSS Custom Properties */
    :global(:root) {
        --background: 0 0% 100%;
        --foreground: 222.2 84% 4.9%;
        --primary: 222.2 47.4% 11.2%;
        --primary-foreground: 210 40% 98%;
        --muted-foreground: 215.4 16.3% 46.9%;
        --border: 214.3 31.8% 91.4%;
        --input: 214.3 31.8% 91.4%;
        --ring: 222.2 84% 4.9%;
    }

    :global(.dark) {
        --background: 222.2 84% 4.9%;
        --foreground: 210 40% 98%;
        --primary: 210 40% 98%;
        --primary-foreground: 222.2 47.4% 11.2%;
        --muted-foreground: 215 20.2% 65.1%;
        --border: 217.2 32.6% 17.5%;
        --input: 217.2 32.6% 17.5%;
        --ring: 212.7 26.8% 83.9%;
    }

    /* Apply design tokens */
    :global(body) {
        background-color: hsl(var(--background));
        color: hsl(var(--foreground));
    }

    /* Button hover effect */
    button:hover {
        background-color: hsl(var(--primary) / 0.9);
    }
</style>