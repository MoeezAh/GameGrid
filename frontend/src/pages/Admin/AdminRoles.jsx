import React, { useState, useEffect } from 'react';
import api from '../../services/api';

const AdminRoles = () => {
  const [roles, setRoles] = useState([]);
  const [permissionGroups, setPermissionGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  // Modal State
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState('create'); // 'create' | 'edit'
  const [selectedRole, setSelectedRole] = useState(null);
  
  // Form State
  const [roleName, setRoleName] = useState('');
  const [roleDescription, setRoleDescription] = useState('');
  const [roleIsActive, setRoleIsActive] = useState(true);
  const [selectedPermissionIds, setSelectedPermissionIds] = useState([]);
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState(null);

  // Delete modal confirmation
  const [roleToDelete, setRoleToDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const fetchRolesAndPermissions = async () => {
    setLoading(true);
    setError(null);
    try {
      const [rolesRes, permsRes] = await Promise.all([
        api.get('/roles'),
        api.get('/roles/permissions')
      ]);
      setRoles(rolesRes.data || []);
      setPermissionGroups(permsRes.data || []);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load roles and permissions.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRolesAndPermissions();
  }, []);

  const openCreateModal = () => {
    setModalMode('create');
    setSelectedRole(null);
    setRoleName('');
    setRoleDescription('');
    setRoleIsActive(true);
    setSelectedPermissionIds([]);
    setFormError(null);
    setShowModal(true);
  };

  const openEditModal = (role) => {
    setModalMode('edit');
    setSelectedRole(role);
    setRoleName(role.name);
    setRoleDescription(role.description || '');
    setRoleIsActive(role.isActive);
    setSelectedPermissionIds(role.permissions ? role.permissions.map(p => p.id) : []);
    setFormError(null);
    setShowModal(true);
  };

  const handleTogglePermission = (id) => {
    if (selectedPermissionIds.includes(id)) {
      setSelectedPermissionIds(selectedPermissionIds.filter(pid => pid !== id));
    } else {
      setSelectedPermissionIds([...selectedPermissionIds, id]);
    }
  };

  const handleToggleCategory = (permissions) => {
    const categoryIds = permissions.map(p => p.id);
    const allSelected = categoryIds.every(id => selectedPermissionIds.includes(id));

    if (allSelected) {
      // Unselect all in this category
      setSelectedPermissionIds(selectedPermissionIds.filter(id => !categoryIds.includes(id)));
    } else {
      // Select all in this category
      const merged = Array.from(new Set([...selectedPermissionIds, ...categoryIds]));
      setSelectedPermissionIds(merged);
    }
  };

  const handleSaveRole = async (e) => {
    e.preventDefault();
    if (!roleName.trim()) {
      setFormError('Role name is required.');
      return;
    }

    setSaving(true);
    setFormError(null);

    const payload = {
      name: roleName.trim(),
      description: roleDescription.trim() || null,
      isActive: roleIsActive,
      permissionIds: selectedPermissionIds
    };

    try {
      if (modalMode === 'create') {
        await api.post('/roles', payload);
        setSuccessMsg(`Role '${payload.name}' created successfully!`);
      } else {
        await api.put(`/roles/${selectedRole.id}`, payload);
        setSuccessMsg(`Role '${payload.name}' updated successfully!`);
      }
      setShowModal(false);
      fetchRolesAndPermissions();
      setTimeout(() => setSuccessMsg(null), 4000);
    } catch (err) {
      setFormError(err.response?.data?.errors?.[0] || err.response?.data?.message || 'Failed to save role.');
    } finally {
      setSaving(false);
    }
  };

  const handleToggleStatus = async (role) => {
    try {
      await api.put(`/roles/${role.id}/status`, { isActive: !role.isActive });
      setSuccessMsg(`Role '${role.name}' is now ${!role.isActive ? 'Active' : 'Inactive'}.`);
      fetchRolesAndPermissions();
      setTimeout(() => setSuccessMsg(null), 3000);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to update role status.');
    }
  };

  const handleDeleteRole = async () => {
    if (!roleToDelete) return;
    setDeleting(true);
    try {
      await api.delete(`/roles/${roleToDelete.id}`);
      setSuccessMsg(`Role '${roleToDelete.name}' deleted successfully.`);
      setRoleToDelete(null);
      fetchRolesAndPermissions();
      setTimeout(() => setSuccessMsg(null), 3000);
    } catch (err) {
      setError(err.response?.data?.errors?.[0] || 'Failed to delete role.');
      setRoleToDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  return (
    <div className="container-fluid p-0">
      {/* Header Banner */}
      <div className="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
        <div>
          <h1 className="h3 mb-1 fw-bold text-gradient display-font">
            <i className="bi bi-shield-shaded me-2 text-primary"></i>
            Role & Permission Management
          </h1>
          <p className="text-secondary mb-0">
            Configure system roles and assign granular application permissions. Super Admin exclusive area.
          </p>
        </div>
        <button className="btn btn-primary d-flex align-items-center gap-2" onClick={openCreateModal}>
          <i className="bi bi-plus-circle-fill"></i>
          <span>Create New Role</span>
        </button>
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

      {/* Roles Table */}
      <div className="card border-0 shadow-sm custom-card">
        <div className="card-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <div className="spinner-border text-primary" role="status"></div>
              <p className="text-secondary mt-2">Loading system roles...</p>
            </div>
          ) : roles.length === 0 ? (
            <div className="text-center py-5 text-secondary">
              <i className="bi bi-shield-slash fs-1 d-block mb-2"></i>
              No roles found. Create one using the button above.
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0 custom-table">
                <thead>
                  <tr>
                    <th style={{ width: '220px' }}>Role Name</th>
                    <th>Description</th>
                    <th style={{ width: '130px' }} className="text-center">Status</th>
                    <th style={{ width: '130px' }} className="text-center">Users</th>
                    <th style={{ width: '150px' }} className="text-center">Permissions</th>
                    <th style={{ width: '160px' }} className="text-end">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {roles.map((role) => (
                    <tr key={role.id}>
                      <td>
                        <div className="d-flex align-items-center gap-2">
                          <span className="fw-bold fs-6">{role.name}</span>
                          {role.isSuperAdmin && (
                            <span className="badge bg-danger text-uppercase" style={{ fontSize: '0.65rem' }}>
                              Super Admin
                            </span>
                          )}
                          {role.isSystemRole && !role.isSuperAdmin && (
                            <span className="badge bg-secondary" style={{ fontSize: '0.65rem' }}>
                              System
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="text-secondary small">
                        {role.description || <span className="fst-italic opacity-50">No description</span>}
                      </td>
                      <td className="text-center">
                        <button
                          className={`btn btn-sm ${role.isActive ? 'btn-outline-success' : 'btn-outline-secondary'} border-0 px-2 py-1`}
                          onClick={() => !role.isSuperAdmin && handleToggleStatus(role)}
                          disabled={role.isSuperAdmin}
                          title={role.isSuperAdmin ? 'Super Admin cannot be deactivated' : 'Click to toggle status'}
                        >
                          <i className={`bi bi-circle-fill me-1 ${role.isActive ? 'text-success' : 'text-secondary'}`} style={{ fontSize: '0.55rem' }}></i>
                          {role.isActive ? 'Active' : 'Inactive'}
                        </button>
                      </td>
                      <td className="text-center">
                        <span className="badge bg-dark-subtle text-body border">
                          <i className="bi bi-people me-1"></i>
                          {role.userCount}
                        </span>
                      </td>
                      <td className="text-center">
                        {role.isSuperAdmin ? (
                          <span className="badge bg-primary-subtle text-primary border border-primary-subtle">
                            All (Wildcard)
                          </span>
                        ) : (
                          <span className="badge bg-secondary-subtle text-secondary border">
                            {role.permissions?.length || 0} granted
                          </span>
                        )}
                      </td>
                      <td className="text-end">
                        <div className="btn-group">
                          <button
                            className="btn btn-sm btn-outline-primary"
                            onClick={() => openEditModal(role)}
                            title="Edit Role & Permissions"
                          >
                            <i className="bi bi-pencil-square me-1"></i>
                            Edit
                          </button>
                          {!role.isSuperAdmin && !role.isSystemRole && (
                            <button
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => setRoleToDelete(role)}
                              title="Delete Role"
                            >
                              <i className="bi bi-trash"></i>
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Create / Edit Modal */}
      {showModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1050 }} tabIndex="-1">
          <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title fw-bold">
                  <i className={`bi ${modalMode === 'create' ? 'bi-shield-plus' : 'bi-shield-gear'} me-2 text-primary`}></i>
                  {modalMode === 'create' ? 'Create New Role' : `Edit Role: ${selectedRole?.name}`}
                </h5>
                <button type="button" className="btn-close" onClick={() => setShowModal(false)}></button>
              </div>

              <form onSubmit={handleSaveRole}>
                <div className="modal-body p-4">
                  {formError && (
                    <div className="alert alert-danger mb-3 py-2">
                      <i className="bi bi-exclamation-circle me-1"></i> {formError}
                    </div>
                  )}

                  <div className="row g-3 mb-4">
                    <div className="col-md-8">
                      <label className="form-label fw-bold small">Role Name *</label>
                      <input
                        type="text"
                        className="form-control"
                        placeholder="e.g. Game Curator, Content Moderator"
                        value={roleName}
                        onChange={(e) => setRoleName(e.target.value)}
                        required
                      />
                    </div>
                    <div className="col-md-4 d-flex align-items-end">
                      <div className="form-check form-switch mb-2">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id="roleActiveSwitch"
                          checked={roleIsActive}
                          onChange={(e) => setRoleIsActive(e.target.checked)}
                          disabled={selectedRole?.isSuperAdmin}
                        />
                        <label className="form-check-label small fw-semibold" htmlFor="roleActiveSwitch">
                          Active Status
                        </label>
                      </div>
                    </div>

                    <div className="col-12">
                      <label className="form-label fw-bold small">Description</label>
                      <textarea
                        className="form-control"
                        rows="2"
                        placeholder="Describe the purpose and responsibilities of this role..."
                        value={roleDescription}
                        onChange={(e) => setRoleDescription(e.target.value)}
                      ></textarea>
                    </div>
                  </div>

                  <hr className="my-3 opacity-25" />

                  {/* Permissions Selection Accordion */}
                  <div className="mb-2">
                    <div className="d-flex align-items-center justify-content-between mb-2">
                      <label className="form-label fw-bold mb-0">
                        <i className="bi bi-key-fill me-1 text-warning"></i>
                        Assign Permissions
                      </label>
                      <span className="badge bg-primary-subtle text-primary border border-primary-subtle">
                        {selectedPermissionIds.length} Selected
                      </span>
                    </div>
                    <p className="text-secondary small mb-3">
                      Select the individual application actions granted to users with this role.
                    </p>

                    {selectedRole?.isSuperAdmin ? (
                      <div className="alert alert-info">
                        <i className="bi bi-info-circle-fill me-2"></i>
                        The <strong>Super Admin</strong> role automatically possesses all current and future permissions across the entire system.
                      </div>
                    ) : (
                      <div className="d-flex flex-column gap-3">
                        {permissionGroups.map((group) => {
                          const groupIds = group.permissions.map(p => p.id);
                          const allSelected = groupIds.every(id => selectedPermissionIds.includes(id));
                          const someSelected = groupIds.some(id => selectedPermissionIds.includes(id)) && !allSelected;

                          return (
                            <div key={group.category} className="card border custom-card p-3">
                              <div className="d-flex align-items-center justify-content-between mb-2 pb-2 border-bottom border-secondary-subtle">
                                <span className="fw-bold text-uppercase small text-primary">
                                  {group.category}
                                </span>
                                <button
                                  type="button"
                                  className="btn btn-sm btn-link p-0 text-decoration-none"
                                  onClick={() => handleToggleCategory(group.permissions)}
                                >
                                  {allSelected ? 'Deselect Category' : 'Select All'}
                                </button>
                              </div>

                              <div className="row g-2">
                                {group.permissions.map((perm) => (
                                  <div key={perm.id} className="col-md-6">
                                    <div 
                                      className={`p-2 rounded border cursor-pointer filter-section-box ${selectedPermissionIds.includes(perm.id) ? 'border-primary bg-primary-subtle' : ''}`}
                                      onClick={() => handleTogglePermission(perm.id)}
                                      style={{ cursor: 'pointer' }}
                                    >
                                      <div className="form-check mb-0">
                                        <input
                                          className="form-check-input"
                                          type="checkbox"
                                          id={`perm_${perm.id}`}
                                          checked={selectedPermissionIds.includes(perm.id)}
                                          onChange={() => {}} // Handled by outer div
                                        />
                                        <label className="form-check-label fw-semibold small d-block" htmlFor={`perm_${perm.id}`}>
                                          {perm.displayName}
                                        </label>
                                        {perm.description && (
                                          <span className="text-secondary d-block" style={{ fontSize: '0.75rem' }}>
                                            {perm.description}
                                          </span>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                ))}
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    )}
                  </div>
                </div>

                <div className="modal-footer border-top border-secondary-subtle">
                  <button type="button" className="btn btn-secondary" onClick={() => setShowModal(false)} disabled={saving}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary d-flex align-items-center gap-2" disabled={saving}>
                    {saving && <span className="spinner-border spinner-border-sm" role="status"></span>}
                    <span>{modalMode === 'create' ? 'Create Role' : 'Save Changes'}</span>
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {roleToDelete && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.7)', zIndex: 1060 }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content custom-card border-0 shadow">
              <div className="modal-header border-bottom border-secondary-subtle">
                <h5 className="modal-title text-danger fw-bold">
                  <i className="bi bi-exclamation-triangle-fill me-2"></i> Delete Role
                </h5>
                <button type="button" className="btn-close" onClick={() => setRoleToDelete(null)}></button>
              </div>
              <div className="modal-body py-4">
                <p className="mb-1">
                  Are you sure you want to permanently delete the role <strong>'{roleToDelete.name}'</strong>?
                </p>
                <p className="text-secondary small mb-0">
                  This action cannot be undone. Any permissions assigned to this role will be removed.
                </p>
              </div>
              <div className="modal-footer border-top border-secondary-subtle">
                <button type="button" className="btn btn-secondary" onClick={() => setRoleToDelete(null)} disabled={deleting}>
                  Cancel
                </button>
                <button type="button" className="btn btn-danger d-flex align-items-center gap-2" onClick={handleDeleteRole} disabled={deleting}>
                  {deleting && <span className="spinner-border spinner-border-sm" role="status"></span>}
                  <span>Delete Role</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminRoles;
