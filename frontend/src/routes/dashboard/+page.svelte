<script>
  console.log('🔥 DASHBOARD PAGE LOADED 🔥');
  
  import { onMount } from 'svelte';
  import { writable } from 'svelte/store';
  import { goto } from '$app/navigation';
  import { browser } from '$app/environment';

  // Only run localStorage checks in the browser
  if (browser) {
    console.log('LocalStorage test:', localStorage ? 'AVAILABLE' : 'NOT AVAILABLE');
    console.log('All localStorage keys:', Object.keys(localStorage));
  }

  // Token management utilities
  const tokenStore = writable(null);
  
  // Store token securely
  function storeToken(token) {
    if (browser) {
      localStorage.setItem('authToken', token);
    }
    tokenStore.set(token);
  }
  
  // Retrieve token
  function getToken() {
    if (!browser) return null;
    
    const token = localStorage.getItem('authToken');
    console.log('Getting token:', token ? 'TOKEN EXISTS' : 'NO TOKEN');
    tokenStore.set(token);
    return token;
  }
  
  // Remove token (logout)
  function removeToken() {
    if (browser) {
      localStorage.removeItem('authToken');
    }
    tokenStore.set(null);
  }
  
  // JWT Debug function
  function debugJWT(token) {
    if (!token) {
      console.error('❌ No token provided');
      return;
    }
    
    try {
      const parts = token.split('.');
      if (parts.length !== 3) {
        console.error('❌ Invalid JWT format - should have 3 parts');
        return;
      }
      
      // Decode header
      const header = JSON.parse(atob(parts[0]));
      console.log('🔍 JWT Header:', header);
      
      // Decode payload
      const payload = JSON.parse(atob(parts[1]));
      console.log('🔍 JWT Payload:', payload);
      
      // Check expiration
      if (payload.exp) {
        const expDate = new Date(payload.exp * 1000);
        const now = new Date();
        console.log('⏰ Expires:', expDate);
        console.log('⏰ Now:', now);
        console.log(expDate > now ? '✅ Token is valid' : '❌ Token expired');
      }
      
      // Check required claims
      console.log('🔍 User ID:', payload.sub || payload.nameid || payload.id || 'NOT FOUND');
      console.log('🔍 Email:', payload.email || 'NOT FOUND');
      
    } catch (error) {
      console.error('❌ Error decoding JWT:', error);
    }
  }
  
  // API base URL - Update this to match your .NET backend port
  const API_BASE_URL = 'http://localhost:5027';
  
  // Test function 1: Basic connection (no auth)
  async function testBasicConnection() {
    try {
      console.log('🧪 Testing basic connection...');
      const response = await fetch(`${API_BASE_URL}/api/dashboard/test`);
      
      console.log('Response status:', response.status);
      console.log('Response headers:', Object.fromEntries(response.headers));
      
      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Basic connection failed:', errorText);
        return;
      }
      
      const data = await response.json();
      console.log('✅ Basic connection works:', data);
    } catch (err) {
      console.error('❌ Basic connection error:', err);
    }
  }

  // Test function 2: With authentication
  async function testWithAuth() {
    try {
      console.log('🧪 Testing with auth...');
      const token = getToken();
      console.log('Token being sent:', token ? token.substring(0, 50) + '...' : 'NO TOKEN');
      
      if (!token) {
        console.error('❌ No token available for auth test');
        return;
      }
      
      const response = await fetch(`${API_BASE_URL}/api/dashboard`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      console.log('Response status:', response.status);
      console.log('Response headers:', Object.fromEntries(response.headers));
      
      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Auth request failed:', errorText);
        console.error('Full response object:', response);
      } else {
        const data = await response.json();
        console.log('✅ Auth request works:', data);
      }
    } catch (err) {
      console.error('❌ Auth request error:', err);
    }
  }
  
  // API call with Authorization header
  async function makeAuthenticatedRequest(endpoint, options = {}) {
    const token = getToken();
    
    console.log('=== DEBUG DASHBOARD REQUEST ===');
    console.log('Token from getToken():', token ? token.substring(0, 50) + '...' : 'NULL');
    console.log('Endpoint:', endpoint);
    console.log('Full URL:', `${API_BASE_URL}${endpoint}`);
    
    if (!token) {
      console.error('❌ No authentication token found');
      throw new Error('No authentication token found');
    }
    
    const url = `${API_BASE_URL}${endpoint}`;
    
    const headers = {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
      ...options.headers
    };
    
    console.log('Request headers:', headers);
    
    const fetchOptions = { 
      ...options, 
      headers: headers
    };
    
    console.log('Making request...');
    
    try {
      const response = await fetch(url, fetchOptions);
      
      console.log('Response status:', response.status);
      console.log('Response statusText:', response.statusText);
      console.log('Response headers:', Object.fromEntries(response.headers));
      
      // Handle token expiration
      if (response.status === 401) {
        console.error('❌ 401 Unauthorized');
        const errorText = await response.text();
        console.error('Server error message:', errorText);
        
        removeToken();
        goto('/login');
        throw new Error('Token expired or invalid');
      }
      
      // Check if response is ok before trying to parse JSON
      if (!response.ok) {
        const errorText = await response.text();
        console.error('❌ Request failed:', errorText);
        throw new Error(`HTTP ${response.status}: ${errorText}`);
      }
      
      console.log('✅ Request successful');
      return response;
      
    } catch (fetchError) {
      console.error('❌ Fetch error:', fetchError);
      throw fetchError;
    }
  }

  // Dashboard state
  let user = null;
  let dashboardData = {
    stats: [],
    recentActivity: [],
    notifications: []
  };
  let loading = true;
  let error = null;

  // Check authentication and load dashboard data
  onMount(async () => {
    console.log('=== DASHBOARD ONMOUNT ===');
    
    const token = getToken();
    
    if (!token) {
      console.log('No token, redirecting to login');
      goto('/login');
      return;
    }
    
    // Debug the JWT token
    debugJWT(token);
    
    try {
      console.log('Making dashboard request...');
      // Fetch dashboard data only
      const dashboardResponse = await makeAuthenticatedRequest('/api/dashboard');
      dashboardData = await dashboardResponse.json();
      console.log('✅ Dashboard data loaded:', dashboardData);
      
      // Set a default user or get from token if needed
      user = { name: 'User' }; // You can decode this from your JWT token if needed
      
    } catch (err) {
      console.error('❌ Dashboard error:', err);
      error = err.message;
    } finally {
      loading = false;
    }
  });

  // Logout function
  function handleLogout() {
    removeToken();
    goto('/login');
  }

  // Refresh dashboard data
  async function refreshDashboard() {
    loading = true;
    try {
      const response = await makeAuthenticatedRequest('/api/dashboard');
      dashboardData = await response.json();
      error = null; // Clear any previous errors
    } catch (err) {
      error = err.message;
    } finally {
      loading = false;
    }
  }
</script>

<!-- Dashboard UI -->
<div class="min-h-screen bg-gray-50">
  <!-- Navigation Header -->
  <nav class="bg-white shadow-sm border-b">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex items-center">
          <h1 class="text-xl font-semibold text-gray-900">Dashboard</h1>
        </div>
        
        <div class="flex items-center space-x-4">
          {#if user}
            <span class="text-sm text-gray-700">Welcome, {user.name}</span>
          {/if}
          <button
            on:click={handleLogout}
            class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
          >
            Logout
          </button>
        </div>
      </div>
    </div>
  </nav>

  <!-- Main Content -->
  <main class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
    <!-- Debug Tools (remove this after fixing) -->
    <div class="mb-6 p-4 bg-yellow-100 border border-yellow-300 rounded">
      <h3 class="text-sm font-bold text-yellow-800 mb-2">🔧 Debug Tools</h3>
      <div class="space-x-2">
        <button 
          on:click={testBasicConnection}
          class="px-3 py-1 bg-blue-500 text-white rounded text-sm hover:bg-blue-600">
          Test Basic Connection
        </button>
        
        <button 
          on:click={testWithAuth}
          class="px-3 py-1 bg-green-500 text-white rounded text-sm hover:bg-green-600">
          Test With Auth
        </button>
        
        <button 
          on:click={() => debugJWT(getToken())}
          class="px-3 py-1 bg-purple-500 text-white rounded text-sm hover:bg-purple-600">
          Debug JWT
        </button>
      </div>
    </div>

    {#if loading}
      <div class="flex items-center justify-center h-64">
        <div class="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-500"></div>
        <p class="ml-4">Loading dashboard...</p>
      </div>
    {:else if error}
      <div class="rounded-md bg-red-50 p-4">
        <div class="flex">
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800">Error loading dashboard</h3>
            <div class="mt-2 text-sm text-red-700">
              <p>{error}</p>
            </div>
            <div class="mt-4">
              <button
                on:click={refreshDashboard}
                class="bg-red-100 px-2 py-1 text-xs font-semibold text-red-800 rounded">
                Try Again
              </button>
            </div>
          </div>
        </div>
      </div>
    {:else}
      <!-- Dashboard Content -->
      <div class="space-y-6">
        <!-- Stats Cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          {#each dashboardData.stats as stat}
            <div class="bg-white overflow-hidden shadow rounded-lg">
              <div class="p-5">
                <div class="flex items-center">
                  <div class="flex-shrink-0">
                    <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                      <span class="text-white text-sm font-bold">{stat.icon}</span>
                    </div>
                  </div>
                  <div class="ml-5 w-0 flex-1">
                    <dl>
                      <dt class="text-sm font-medium text-gray-500 truncate">{stat.title}</dt>
                      <dd class="text-lg font-medium text-gray-900">{stat.value}</dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>
          {/each}
        </div>

        <!-- Content Grid -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Recent Activity -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Recent Activity</h3>
              <div class="space-y-3">
                {#each dashboardData.recentActivity as activity}
                  <div class="flex items-center space-x-3">
                    <div class="w-2 h-2 bg-blue-400 rounded-full"></div>
                    <div class="flex-1">
                      <p class="text-sm text-gray-900">{activity.action}</p>
                      <p class="text-xs text-gray-500">{activity.timestamp}</p>
                    </div>
                  </div>
                {/each}
              </div>
            </div>
          </div>

          <!-- Notifications -->
          <div class="bg-white shadow rounded-lg">
            <div class="px-4 py-5 sm:p-6">
              <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Notifications</h3>
              <div class="space-y-3">
                {#each dashboardData.notifications as notification}
                  <div class="border-l-4 border-yellow-400 bg-yellow-50 p-4">
                    <div class="flex">
                      <div class="ml-3">
                        <p class="text-sm text-yellow-700">{notification.message}</p>
                        <p class="text-xs text-yellow-600 mt-1">{notification.time}</p>
                      </div>
                    </div>
                  </div>
                {/each}
              </div>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-4 py-5 sm:p-6">
            <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Quick Actions</h3>
            <div class="flex flex-wrap gap-3">
              <button
                on:click={refreshDashboard}
                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                Refresh Data
              </button>
              
              <button
                on:click={async () => {
                  try {
                    const response = await makeAuthenticatedRequest('/api/export');
                    console.log('Export initiated');
                  } catch (err) {
                    console.error('Export failed:', err);
                  }
                }}
                class="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                Export Data
              </button>
            </div>
          </div>
        </div>
      </div>
    {/if}
  </main>
</div>

<style>
  /* Custom animations */
  @keyframes spin {
    to { transform: rotate(360deg); }
  }
  
  .animate-spin {
    animation: spin 1s linear infinite;
  }
</style>