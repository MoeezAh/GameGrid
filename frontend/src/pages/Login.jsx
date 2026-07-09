import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  
  const [usernameOrEmail, setUsernameOrEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!usernameOrEmail || !password) {
      setError('Please fill in all fields.');
      return;
    }
    
    setError('');
    setSubmitting(true);
    
    const result = await login(usernameOrEmail, password);
    setSubmitting(false);
    
    if (result.success) {
      navigate('/');
    } else {
      setError(result.message);
    }
  };

  return (
    <div className="auth-page-bg d-flex align-items-center justify-content-center p-3">
      <div className="glass-panel p-4 p-sm-5 w-100" style={{ maxWidth: '450px' }}>
        <div className="text-center mb-4">
          <i 
            className="bi bi-controller fs-1 mb-2 d-inline-block" 
            style={{ background: 'linear-gradient(135deg, var(--accent-purple), var(--accent-teal))', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}
          ></i>
          <h2 className="display-font fw-bold" style={{ color: 'var(--text-primary)' }}>Welcome Back</h2>
          <p className="text-muted small">Manage your personal game library</p>
        </div>

        {error && (
          <div className="alert-theme-danger d-flex align-items-center gap-2 mb-3" role="alert">
            <i className="bi bi-exclamation-triangle-fill flex-shrink-0"></i>
            <span>{error}</span>
          </div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label small fw-bold uppercase">Username or Email</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-envelope-fill"></i></span>
              <input 
                type="text" 
                className="form-control form-glass-control"
                placeholder="Enter username or email"
                value={usernameOrEmail}
                onChange={(e) => setUsernameOrEmail(e.target.value)}
              />
            </div>
          </div>

          <div className="mb-4">
            <label className="form-label small fw-bold uppercase">Password</label>
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-shield-lock-fill"></i></span>
              <input 
                type="password" 
                className="form-control form-glass-control"
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
          </div>

          <button 
            type="submit" 
            className="btn btn-premium-purple w-100 py-2 fw-bold d-flex align-items-center justify-content-center gap-2"
            disabled={submitting}
          >
            {submitting ? (
              <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            ) : (
              <>
                <span>Sign In</span>
                <i className="bi bi-arrow-right-short fs-5"></i>
              </>
            )}
          </button>
        </form>

        <div className="text-center mt-4">
          <p className="text-muted small mb-3">
            Don't have an account?{' '}
            <Link to="/register" className="text-theme-accent fw-bold text-decoration-none">Register</Link>
          </p>
          <div className="auth-card-hint p-3 rounded-3 text-start small">
            <strong>Seed accounts available:</strong>
            <ul className="mb-0 ps-3 mt-1">
              <li>Admin: <code>admin</code> / <code>Admin123!</code></li>
              <li>User: <code>user</code> / <code>User123!</code></li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
