import React, { useState, useEffect } from 'react';
import { Link, NavLink, Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Layout = () => {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'dark');
  const [mobileSidebarShow, setMobileSidebarShow] = useState(false);

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme(theme === 'dark' ? 'light' : 'dark');
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getPageTitle = () => {
    const path = location.pathname;
    if (path === '/') return 'Dashboard';
    if (path === '/library') return 'My Library';
    if (path === '/games/add') return 'Add New Game';
    if (path.startsWith('/games/edit')) return 'Edit Game';
    if (path.startsWith('/games/')) return 'Game Details';
    if (path === '/profile') return 'User Profile';
    if (path.startsWith('/admin')) {
      const parts = path.split('/');
      const section = parts[parts.length - 1];
      return `Manage ${section.charAt(0).toUpperCase() + section.slice(1)}`;
    }
    return 'Game Catalog';
  };

  return (
    <div className="app-shell">
      {/* Sidebar navigation */}
      <aside className={`sidebar ${mobileSidebarShow ? 'show' : ''}`}>
        <div className="sidebar-header">
          <div className="sidebar-logo">
            <i className="bi bi-controller"></i>
            <span className="h4 mb-0 fw-bold display-font logo-name">ANTIGRAVITY</span>
          </div>
        </div>

        <div className="flex-grow-1 overflow-y-auto py-2">
          <div className="nav-section-title">Library</div>
          <NavLink to="/" className="sidebar-nav-link" end onClick={() => setMobileSidebarShow(false)}>
            <i className="bi bi-grid-1x2-fill"></i>
            <span>Dashboard</span>
          </NavLink>
          <NavLink to="/library" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
            <i className="bi bi-collection-play-fill"></i>
            <span>My Library</span>
          </NavLink>
          <NavLink to="/games/add" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
            <i className="bi bi-plus-circle-fill"></i>
            <span>Add Game</span>
          </NavLink>

          <div className="nav-section-title">Account</div>
          <NavLink to="/profile" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
            <i className="bi bi-person-badge-fill"></i>
            <span>Profile</span>
          </NavLink>

          {isAdmin() && (
            <>
              <div className="nav-section-title">Administration</div>
              <NavLink to="/admin/platforms" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-pc-display"></i>
                <span>Platforms</span>
              </NavLink>
              <NavLink to="/admin/services" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-shop"></i>
                <span>Services</span>
              </NavLink>
              <NavLink to="/admin/developers" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-code-slash"></i>
                <span>Developers</span>
              </NavLink>
              <NavLink to="/admin/publishers" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-building"></i>
                <span>Publishers</span>
              </NavLink>
              <NavLink to="/admin/genres" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-tags-fill"></i>
                <span>Genres</span>
              </NavLink>
              <NavLink to="/admin/tags" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-hash"></i>
                <span>Tags</span>
              </NavLink>
              <NavLink to="/admin/themes" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-palette-fill"></i>
                <span>Themes</span>
              </NavLink>
              <NavLink to="/admin/franchises" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-diagram-3-fill"></i>
                <span>Franchises</span>
              </NavLink>
              <NavLink to="/admin/series" className="sidebar-nav-link" onClick={() => setMobileSidebarShow(false)}>
                <i className="bi bi-layers-half"></i>
                <span>Series</span>
              </NavLink>
            </>
          )}
        </div>

        {/* User Card footer */}
        <div className="pt-3" style={{ borderTop: '1px solid var(--border-color)' }}>
          <div className="d-flex align-items-center justify-content-between px-2 mb-3">
            <div className="d-flex align-items-center gap-2">
              <div 
                className="rounded-circle d-flex align-items-center justify-content-center fw-bold"
                style={{ 
                  width: '40px', height: '40px',
                  background: 'linear-gradient(135deg, var(--accent-purple), var(--accent-teal))',
                  color: '#ffffff',
                  border: '2px solid var(--accent-purple)'
                }}
              >
                {user?.username ? user.username.charAt(0).toUpperCase() : 'U'}
              </div>
              <div style={{ maxWidth: '120px' }}>
                <div className="text-truncate fw-bold small sidebar-user-name">{user?.username}</div>
                <div className="text-truncate sidebar-user-email" style={{ fontSize: '0.7rem' }}>{user?.email}</div>
              </div>
            </div>
            
            <button className="btn btn-sm btn-outline-danger border-0 p-2" onClick={handleLogout} title="Log Out">
              <i className="bi bi-box-arrow-right fs-5"></i>
            </button>
          </div>
        </div>
      </aside>

      {/* Main container panel */}
      <div className="main-content d-flex flex-column min-vh-100 p-0">
        {/* Header toolbar */}
        <header className="top-header">
          <div className="d-flex align-items-center gap-3">
            <button 
              className="btn d-lg-none"
              style={{ background: 'var(--bg-tertiary)', border: '1px solid var(--border-color)', color: 'var(--text-primary)' }}
              onClick={() => setMobileSidebarShow(!mobileSidebarShow)}
            >
              <i className="bi bi-list fs-4"></i>
            </button>
            <h2 className="mb-0 display-font page-title">{getPageTitle()}</h2>
          </div>

          <div className="d-flex align-items-center gap-3">
            {/* Theme switcher */}
            <button 
              className="theme-toggle-btn"
              onClick={toggleTheme}
              title="Toggle Theme"
            >
              {theme === 'dark' 
                ? <i className="bi bi-sun-fill" style={{ color: '#fbbf24' }}></i> 
                : <i className="bi bi-moon-fill" style={{ color: 'var(--accent-purple)' }}></i>
              }
            </button>
            
            <Link to="/profile" className="text-decoration-none d-none d-sm-block">
              <span className="role-badge">
                <i className="bi bi-shield-lock-fill"></i>
                {user?.roles?.includes('Administrator') ? 'Admin' : 'User'}
              </span>
            </Link>
          </div>
        </header>

        {/* Content Outlet body */}
        <main className="flex-grow-1 p-3 p-md-4 p-lg-5 container-fluid fade-in">
          <Outlet />
        </main>
      </div>

      {/* Mobile Sidebar backdrop */}
      {mobileSidebarShow && (
        <div 
          className="position-fixed top-0 start-0 w-100 h-100 bg-black bg-opacity-75 z-3 d-lg-none" 
          onClick={() => setMobileSidebarShow(false)}
        ></div>
      )}
    </div>
  );
};

export default Layout;
