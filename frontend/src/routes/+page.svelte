<script>
    import { onMount } from 'svelte';
    
    let status = "Checking database connection...";
    
    onMount(async () => {
        try {
            const res = await fetch('http://localhost:5027/api/health');
            const data = await res.json();
            status = data.message;
        } catch (err) {
            status = "Could not connect to the server.";
        }
    });
</script>

<div class="min-h-screen bg-gradient-to-br from-purple-50 to-blue-100 flex items-center justify-center p-4">
    <div class="bg-white rounded-xl shadow-2xl p-8 w-full max-w-lg text-center">
        <h1 class="text-4xl font-bold text-gray-900 mb-4">Welcome to SvelteKit</h1>
        <p class="text-gray-600 mb-6">Visit <a href="https://svelte.dev/docs/kit" class="text-blue-600 hover:text-blue-800">svelte.dev/docs/kit</a> to read the documentation</p>
        
        <!-- Database Status -->
        <div class="mb-8">
            <h2 class="text-xl font-semibold text-gray-800 mb-2">Database Status</h2>
            <p class="text-lg {status.includes('successfully') ? 'text-green-600' : 'text-red-600'}">{status}</p>
        </div>
        
        <!-- Navigation Buttons -->
        <div class="space-y-4">
            <a 
                href="/register" 
                class="block w-full bg-indigo-600 text-white py-3 px-6 rounded-lg font-medium hover:bg-indigo-700 transition-colors"
            >
                Create Account
            </a>
            
            <a 
                href="/login" 
                class="block w-full bg-gray-600 text-white py-3 px-6 rounded-lg font-medium hover:bg-gray-700 transition-colors"
            >
                Sign In
            </a>
        </div>
    </div>
</div>