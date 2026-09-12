import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ProtectedRoute, PermissionRoute, SuperAdminRoute, AdminRoute, AuthRoute } from './components/RouteGuards';

// Layout & Core components
import Layout from './components/Layout';

// Page components
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Catalog from './pages/Catalog';
import Library from './pages/Library';
import GameForm from './pages/GameForm';
import GameDetail from './pages/GameDetail';
import Profile from './pages/Profile';
import MyGameRequests from './pages/GameRequests/MyGameRequests';
import AdminGameRequests from './pages/Admin/AdminGameRequests';
import AdminRoles from './pages/Admin/AdminRoles';
import AdminUsers from './pages/Admin/AdminUsers';
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
            
            {/* Central Game Catalog */}
            <Route path="catalog" element={<Catalog />} />

            {/* Personal Game Library */}
            <Route path="library" element={<Library />} />
            
            {/* Game Requests Workflow */}
            <Route path="game-requests" element={<MyGameRequests />} />

            {/* Game Management (Catalog Add / Edit) */}
            <Route 
              path="games/add" 
              element={
                <PermissionRoute permission="Games.Create">
                  <GameForm />
                </PermissionRoute>
              } 
            />
            <Route 
              path="games/edit/:id" 
              element={
                <PermissionRoute permission="Games.Edit">
                  <GameForm />
                </PermissionRoute>
              } 
            />
            <Route path="games/:id" element={<GameDetail />} />
            
            {/* Account Settings profile */}
            <Route path="profile" element={<Profile />} />

            {/* Admin: Game Requests Review */}
            <Route 
              path="admin/game-requests" 
              element={
                <PermissionRoute anyPermissions={['GameRequests.Review', 'GameRequests.Approve', 'GameRequests.Reject']}>
                  <AdminGameRequests />
                </PermissionRoute>
              } 
            />

            {/* Admin: Role Management (Super Admin Exclusive) */}
            <Route 
              path="admin/roles" 
              element={
                <SuperAdminRoute>
                  <AdminRoles />
                </SuperAdminRoute>
              } 
            />

            {/* Admin: User Management */}
            <Route 
              path="admin/users" 
              element={
                <PermissionRoute anyPermissions={['Users.View', 'Users.ManageRoles']}>
                  <AdminUsers />
                </PermissionRoute>
              } 
            />

            {/* Administrator Taxonomy & Metadata settings */}
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

