import React, { useState, useEffect } from 'react';
import api from '../../services/api';

const AdminUsers = () => {
  const [users, setUsers] = useState([]);
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  // Manage Roles Modal State
  const [selectedUser, setSelectedUser] = useState(null);
  const [selectedRoleIds, setSelectedRoleIds] = useState([]);
  const [saving, setSaving] = useState(false);
  const [modalError, setModalError] = useState(null);

  const fetchUsersAndRoles = async () => {
    setLoading(true);
    setError(null);
    try {
      const [usersRes, rolesRes] = await Promise.all([
        api.get('/users'),
        api.get('/roles')
      ]);
      setUsers(usersRes.data || []);
      setRoles(rolesRes.data || []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load user directory.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsersAndRoles();
  }, []);

  const openManageRolesModal = (user) => {
    setSelectedUser(user);
    setSelectedRoleIds([...(user.roleIds || [])]);
    setModalError(null);
  };

  const handleToggleRole = (roleId) => {
    if (selectedRoleIds.includes(roleId)) {
      setSelectedRoleIds(selectedRoleIds.filter(id => id !== roleId));
    } else {
      setSelectedRoleIds([...selectedRoleIds, roleId]);
    }
  };

  const handleSaveRoles = async (e) => {
    e.preventDefault();
    if (selectedRoleIds.length === 0) {
      setModalError('Every user must have at least one role assigned.');
      return;
    }

    setSaving(true);
    setModalError(null);

    try {
      await api.put(`/users/${selectedUser.userId}/roles`, { roleIds: selectedRoleIds });
      setSuccessMsg(`Roles for '${selectedUser.username}' updated successfully!`);
      setSelectedUser(null);
      fetchUsersAndRoles();
      setTimeout(() => setSuccessMsg(null), 4000);
    } catch (err) {
      setModalError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to assign roles.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="container-fluid p-0">
      {/* Header */}
      <div className="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
        <div>
          <h1 className="h3 mb-1 fw-bold text-gradient display-font">
            <i className="bi bi-people-fill me-2 text-primary"></i>
            User & Role Assignments
          </h1>
          <p className="text-secondary mb-0">
            View registered user accounts and assign multiple roles to customize access privileges.
          </p>
        </div>
      </div>

      {/* Notifications */}
      {successMsg && (
        <div className="alert alert-success d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-check-circle-fill fs-5"></i>
          <div>{successMsg}</div>
        </div>
      )}
      {error && (
        <div className="alert alert-danger d-flex align-items-center gap-2 mb-4">
          <i className="bi bi-exclamation-triangle-fill fs-5"></i>
          <div>{error}</div>
        </div>
      )}

      {/* Users Table */}
      <div className="card border-0 shadow-sm custom-card">
        <div className="card-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-primary" role="status"></div>
              <p className="text-secondary mt-2">Loading user directory...</p>
            </div>
          ) : users.length === 0 ? (
            <div className="text-center py-5 text-secondary">
              <i className="bi bi-person-x fs-1 d-block mb-2"></i>
              No users found.
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0 custom-table">
                <thead>
                  <tr>
                    <th style={{ width: '220px' }}>User</th>
                    <th>Email</th>
                    <th>Assigned Roles</th>
                    <th style={{ width: '160px' }} className="text-end">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((user) => (
                    <tr key={user.userId}>
                      <td>
                        <div className="d-flex align-items-center gap-2">
                          <div 
                            className="rounded-circle d-flex align-items-center justify-content-center fw-bold small text-white"
                            style={{ 
                              width: '34px', height: '34px',
                              background: 'linear-gradient(135deg, var(--accent-purple), var(--accent-teal))'
                            }}
                          >
                            {user.username.charAt(0).toUpperCase()}
                          </div>
                          <div>
                            <div className="fw-bold">{user.username}</div>
                            {user.isSuperAdmin && (
                              <span className="badge bg-danger text-uppercase" style={{ fontSize: '0.65rem' }}>
                                Super Admin
                              </span>
                            )}
                          </div>
                        </div>
                      </td>
                      <td className="text-secondary small">{user.email}</td>
                      <td>
                        <div className="d-flex flex-wrap gap-1">
                          {user.roleNames && user.roleNames.length > 0 ? (
                            user.roleNames.map((rName, idx) => (
                              <span key={idx} className="badge bg-primary-subtle text-primary border border-primary-subtle">
                                <i className="bi bi-shield-check me-1"></i>
                                {rName}
                              </span>
                            ))
                          ) : (
                            <span className="badge bg-warning-subtle text-warning border border-warning-subtle">
                              No Roles (Needs Assignment)
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="text-end">
                        <button
                          className="btn btn-sm btn-outline-primary d-inline-flex align-items-center gap-1"
                          onClick={() => openManageRolesModal(user)}
                          title="Assign or Edit User Roles"
                        >
                          <i className="bi bi-shield-plus"></i>
                          <span>Manage Roles</span>
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Manage Roles Modal */}
      {selectedUser && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className="bi bi-shield-lock me-2 text-primary"></i>
                  Manage Roles: <span className="text-primary">{selectedUser.username}</span>
                </h5>
                <button type="button" className="btn-close" onClick={() => setSelectedUser(null)}></button>
              </div>

              <form onSubmit={handleSaveRoles}>
                <div className="modal-body p-4">
                  {modalError && (
                    <div className="alert alert-danger py-2 mb-3">
                      <i className="bi bi-exclamation-circle me-1"></i> {modalError}
                    </div>
                  )}

                  <p className="text-secondary small mb-3">
                    A user may have one or multiple roles. Effective permissions will be the union of all assigned roles.
                  </p>

                  <div className="d-flex flex-column gap-2">
                    {roles.map((role) => (
                      <div
                        key={role.id}
                        className={`p-3 rounded border cursor-pointer filter-section-box ${selectedRoleIds.includes(role.id) ? 'border-primary bg-primary-subtle' : ''}`}
                        onClick={() => handleToggleRole(role.id)}
                        style={{ cursor: 'pointer' }}
                      >
                        <div className="form-check mb-0">
                          <input
                            className="form-check-input"
                            type="checkbox"
                            id={`user_role_${role.id}`}
                            checked={selectedRoleIds.includes(role.id)}
                            onChange={() => {}} // Handled by outer div
                          />
                          <label className="form-check-label fw-bold d-flex align-items-center gap-2" htmlFor={`user_role_${role.id}`}>
                            <span>{role.name}</span>
                            {role.isSuperAdmin && (
                              <span className="badge bg-danger" style={{ fontSize: '0.65rem' }}>Super Admin</span>
                            )}
                            {!role.isActive && (
                              <span className="badge bg-secondary" style={{ fontSize: '0.65rem' }}>Inactive</span>
                            )}
                          </label>
                          {role.description && (
                            <div className="text-secondary small mt-1" style={{ fontSize: '0.78rem' }}>
                              {role.description}
                            </div>
                          )}
                          <div className="text-secondary opacity-75 mt-1" style={{ fontSize: '0.72rem' }}>
                            {role.permissions?.length || 0} permissions included
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                <div className="modal-footer border-top border-secondary-subtle">
                  <button type="button" className="btn btn-secondary" onClick={() => setSelectedUser(null)} disabled={saving}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary d-flex align-items-center gap-2" disabled={saving}>
                    {saving && <span className="spinner-border spinner-border-sm" role="status"></span>}
                    <span>Save Role Assignments</span>
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminUsers;
