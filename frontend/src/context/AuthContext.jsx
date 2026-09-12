import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Restore user session from local storage
    const storedUser = localStorage.getItem('user');
    const storedToken = localStorage.getItem('token');
    
    if (storedUser && storedToken) {
      try {
        setUser(JSON.parse(storedUser));
      } catch (e) {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
      }
    }
    setLoading(false);
  }, []);

  const login = async (usernameOrEmail, password) => {
    try {
      const response = await api.post('/auth/login', { usernameOrEmail, password });
      const authData = response.data;
      
      localStorage.setItem('token', authData.token);
      
      const userData = {
        username: authData.username,
        email: authData.email,
        roles: authData.roles || [],
        permissions: authData.permissions || [],
        isSuperAdmin: !!authData.isSuperAdmin
      };
      
      localStorage.setItem('user', JSON.stringify(userData));
      setUser(userData);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.errors?.[0] || error.message || 'Login failed. Please check credentials.'
      };
    }
  };

  const register = async (username, email, password) => {
    try {
      const response = await api.post('/auth/register', { username, email, password });
      const authData = response.data;
      
      localStorage.setItem('token', authData.token);
      
      const userData = {
        username: authData.username,
        email: authData.email,
        roles: authData.roles || [],
        permissions: authData.permissions || [],
        isSuperAdmin: !!authData.isSuperAdmin
      };
      
      localStorage.setItem('user', JSON.stringify(userData));
      setUser(userData);
      return { success: true };
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.errors?.[0] || error.message || 'Registration failed.'
      };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  const refreshProfile = async () => {
    try {
      const response = await api.get('/auth/profile');
      const profile = response.data;
      const updatedUser = {
        ...user,
        username: profile.username,
        email: profile.email,
        roles: profile.roles || [],
        permissions: profile.permissions || [],
        isSuperAdmin: !!profile.isSuperAdmin
      };
      localStorage.setItem('user', JSON.stringify(updatedUser));
      setUser(updatedUser);
    } catch (err) {
      console.error('Failed to refresh user profile', err);
    }
  };

  // Permission evaluation helpers
  const isSuperAdmin = () => {
    return !!user?.isSuperAdmin;
  };

  const hasPermission = (permissionName) => {
    if (!user) return false;
    if (user.isSuperAdmin) return true;
    return user.permissions?.includes(permissionName) || false;
  };

  const hasAnyPermission = (permissionNames = []) => {
    if (!user) return false;
    if (user.isSuperAdmin) return true;
    return permissionNames.some(p => user.permissions?.includes(p));
  };

  const hasRole = (roleName) => {
    if (!user || !user.roles) return false;
    return user.roles.includes(roleName);
  };

  // Backward compatibility alias
  const isAdmin = () => {
    return isSuperAdmin() || hasRole('Administrator') || hasRole('Admin');
  };

  const value = {
    user,
    loading,
    login,
    register,
    logout,
    refreshProfile,
    isSuperAdmin,
    hasPermission,
    hasAnyPermission,
    hasRole,
    isAdmin
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  return useContext(AuthContext);
};
