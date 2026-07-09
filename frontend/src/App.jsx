import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute, AdminRoute, AuthRoute } from './components/RouteGuards';

// Layout & Core components
import Layout from './components/Layout';

// Page components
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Library from './pages/Library';
import GameForm from './pages/GameForm';
import GameDetail from './pages/GameDetail';
import Profile from './pages/Profile';
import AdminMetadata from './pages/Admin/AdminMetadata';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Public Auth Routes */}
          <Route 
            path="/login" 
            element={
              <AuthRoute>
                <Login />
              </AuthRoute>
            } 
          />
          <Route 
            path="/register" 
            element={
              <AuthRoute>
                <Register />
              </AuthRoute>
            } 
          />

          {/* Protected Library Routes inside Shell layout */}
          <Route 
            path="/" 
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            {/* Dashboard page */}
            <Route index element={<Dashboard />} />
            
            {/* Catalog collection pages */}
            <Route path="library" element={<Library />} />
            <Route path="games/add" element={<GameForm />} />
            <Route path="games/edit/:id" element={<GameForm />} />
            <Route path="games/:id" element={<GameDetail />} />
            
            {/* Account Settings profile */}
            <Route path="profile" element={<Profile />} />

            {/* Administrator Tables settings */}
            <Route 
              path="admin/:type" 
              element={
                <AdminRoute>
                  <AdminMetadata />
                </AdminRoute>
              } 
            />
          </Route>

          {/* Catch-all Fallback Redirect */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
