import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import api from '../../services/api';

const metadataConfig = {
  platforms: {
    title: 'Platforms',
    endpoint: 'platforms',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'manufacturer', label: 'Manufacturer', sortable: true },
      { key: 'releaseDate', label: 'Release Date', sortable: true, format: (val) => val ? new Date(val).toLocaleDateString() : '-' },
      { key: 'generation', label: 'Gen', sortable: true }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'manufacturer', label: 'Manufacturer', type: 'text' },
      { name: 'releaseDate', label: 'Release Date', type: 'date' },
      { name: 'generation', label: 'Generation', type: 'number' },
      { name: 'notes', label: 'Notes', type: 'textarea' }
    ]
  },
  services: {
    title: 'Digital Services',
    endpoint: 'services',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'website', label: 'Website', format: (val) => val ? <a href={val} target="_blank" rel="noreferrer" className="text-teal-400">{val}</a> : '-' },
      { key: 'notes', label: 'Notes' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'website', label: 'Website URL', type: 'url' },
      { name: 'notes', label: 'Notes', type: 'textarea' }
    ]
  },
  developers: {
    title: 'Developers',
    endpoint: 'developers',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'country', label: 'Country', sortable: true },
      { key: 'website', label: 'Website', format: (val) => val ? <a href={val} target="_blank" rel="noreferrer" className="text-teal-400">{val}</a> : '-' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'website', label: 'Website URL', type: 'url' },
      { name: 'country', label: 'Country', type: 'text' },
      { name: 'foundedDate', label: 'Founded Date', type: 'date' },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },
  publishers: {
    title: 'Publishers',
    endpoint: 'publishers',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'country', label: 'Country', sortable: true },
      { key: 'website', label: 'Website', format: (val) => val ? <a href={val} target="_blank" rel="noreferrer" className="text-teal-400">{val}</a> : '-' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'website', label: 'Website URL', type: 'url' },
      { name: 'country', label: 'Country', type: 'text' },
      { name: 'foundedDate', label: 'Founded Date', type: 'date' },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },
  genres: {
    title: 'Genres',
    endpoint: 'genres',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'description', label: 'Description' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },
  tags: {
    title: 'Tags',
    endpoint: 'tags',
    columns: [
      { key: 'name', label: 'Name', sortable: true }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true }
    ]
  },
  themes: {
    title: 'Themes',
    endpoint: 'themes',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'description', label: 'Description' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },
  franchises: {
    title: 'Franchises',
    endpoint: 'franchises',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'description', label: 'Description' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },
  series: {
    title: 'Series',
    endpoint: 'series',
    columns: [
      { key: 'name', label: 'Name', sortable: true },
      { key: 'description', label: 'Description' }
    ],
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  }
};

const AdminMetadata = () => {
  const { type } = useParams();
  const config = metadataConfig[type];

  if (!config) {
    return <div className="text-theme-secondary">Category '{type}' not found.</div>;
  }

  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [sortBy, setSortBy] = useState('name');
  const [sortOrder, setSortOrder] = useState('asc');

  // Form states
  const [formData, setFormData] = useState({});
  const [editingId, setEditingId] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const fetchItems = async () => {
    setLoading(true);
    try {
      const response = await api.get(`/metadata/${config.endpoint}`, {
        params: {
          pageNumber: page,
          pageSize,
          search,
          sortBy,
          sortOrder
        }
      });
      setItems(response.data.items || []);
      setTotalCount(response.data.totalCount || 0);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setPage(1);
    fetchItems();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [type, search, sortBy, sortOrder]);

  useEffect(() => {
    fetchItems();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleOpenCreate = () => {
    setFormData({});
    setEditingId(null);
    setError('');
    setShowModal(true);
  };

  const handleOpenEdit = (item) => {
    const editData = {};
    config.fields.forEach(f => {
      let val = item[f.name] || '';
      if (f.type === 'date' && val) {
        val = val.substring(0, 10); // Format YYYY-MM-DD
      }
      editData[f.name] = val;
    });
    setFormData(editData);
    setEditingId(item.id);
    setError('');
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');

    // Prepare payload
    const payload = { ...formData };
    config.fields.forEach(f => {
      if (f.type === 'number' && payload[f.name]) {
        payload[f.name] = parseInt(payload[f.name]);
      } else if (f.type === 'date' && payload[f.name]) {
        payload[f.name] = new Date(payload[f.name]).toISOString();
      }
    });

    try {
      if (editingId) {
        await api.put(`/metadata/${config.endpoint}/${editingId}`, { id: editingId, ...payload });
      } else {
        await api.post(`/metadata/${config.endpoint}`, payload);
      }
      setShowModal(false);
      fetchItems();
    } catch (err) {
      setError(err.message || 'Failed to save metadata.');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this item?')) return;
    try {
      await api.delete(`/metadata/${config.endpoint}/${id}`);
      fetchItems();
    } catch (err) {
      alert(err.message || 'Failed to delete metadata.');
    }
  };

  const handleSort = (key) => {
    if (sortBy === key) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(key);
      setSortOrder('asc');
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="container-fluid py-2">
      <div className="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-3 mb-4">
        <div>
          <h3 className="display-font mb-1">Manage {config.title}</h3>
          <p className="text-muted small mb-0">Total: {totalCount} records</p>
        </div>
        <button className="btn btn-premium-purple d-flex align-items-center gap-2" onClick={handleOpenCreate}>
          <i className="bi bi-plus-lg"></i>
          <span>Add New</span>
        </button>
      </div>

      {/* Grid Filter Actions */}
      <div className="glass-panel p-3 mb-4">
        <div className="row g-2">
          <div className="col-12 col-md-4">
            <div className="input-group">
              <span className="input-group-addon"><i className="bi bi-search"></i></span>
              <input
                type="text"
                className="form-control form-glass-control"
                placeholder={`Search ${config.title.toLowerCase()}...`}
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </div>
          </div>
        </div>
      </div>

      {/* Table Data list */}
      <div className="glass-panel p-0 overflow-hidden mb-4">
        <div className="table-responsive">
          <table className="table table-theme mb-0 align-middle">
            <thead style={{ borderBottom: '1px solid var(--border-color)' }}>
              <tr>
                {config.columns.map((col, idx) => (
                  <th
                    key={idx}
                    className={`p-3 ${col.sortable ? 'cursor-pointer' : ''}`}
                    onClick={() => col.sortable && handleSort(col.key)}
                  >
                    <div className="d-flex align-items-center gap-2">
                      <span>{col.label}</span>
                      {col.sortable && sortBy === col.key && (
                        <i className={`bi bi-sort-${sortOrder === 'asc' ? 'alpha-down' : 'alpha-up'}`} style={{ color: 'var(--accent-purple)' }}></i>
                      )}
                    </div>
                  </th>
                ))}
                <th className="p-3 text-end" style={{ width: '120px' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={config.columns.length + 1} className="p-5 text-center text-muted">
                    <div className="spinner-border mb-2" style={{ color: 'var(--accent-purple)' }} role="status"></div>
                    <div>Loading records...</div>
                  </td>
                </tr>
              ) : items.length === 0 ? (
                <tr>
                  <td colSpan={config.columns.length + 1} className="p-5 text-center text-muted">
                    <i className="bi bi-folder-x fs-1 mb-2 d-block"></i>
                    No items found matching the filter criteria.
                  </td>
                </tr>
              ) : (
                items.map((item) => (
                  <tr key={item.id}>
                    {config.columns.map((col, idx) => (
                      <td key={idx} className="p-3" style={{ color: 'var(--text-secondary)' }}>
                        {col.format ? col.format(item[col.key]) : item[col.key]}
                      </td>
                    ))}
                    <td className="p-3 text-end">
                      <div className="d-flex justify-content-end gap-1">
                        <button className="btn btn-sm btn-outline-info border-0 p-1" onClick={() => handleOpenEdit(item)}>
                          <i className="bi bi-pencil-fill"></i>
                        </button>
                        <button className="btn btn-sm btn-outline-danger border-0 p-1" onClick={() => handleDelete(item.id)}>
                          <i className="bi bi-trash-fill"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Pagination Controls */}
      {totalPages > 1 && (
        <div className="d-flex justify-content-between align-items-center mb-4">
          <span className="text-muted small">
            Showing Page {page} of {totalPages}
          </span>
          <nav>
            <div className="d-flex gap-1">
              <button className="page-btn" onClick={() => setPage(page - 1)} disabled={page === 1}>Previous</button>
              <button className="page-btn" onClick={() => setPage(page + 1)} disabled={page === totalPages}>Next</button>
            </div>
          </nav>
        </div>
      )}

      {/* Modals Form Dialog */}
      {showModal && (
        <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.75)' }} tabIndex="-1">
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content glass-panel" style={{ border: '1px solid var(--border-color)' }}>
              <div className="modal-header p-3" style={{ borderBottom: '1px solid var(--border-color)' }}>
                <h5 className="modal-title display-font">
                  {editingId ? 'Edit Item' : `Add New ${config.title.slice(0, -1)}`}
                </h5>
                <button type="button" className="btn-close" style={{ filter: 'var(--bs-btn-close-filter, none)' }} onClick={() => setShowModal(false)}></button>
              </div>
              <form onSubmit={handleSubmit}>
                <div className="modal-body p-4">
                  {error && (
                    <div className="alert-theme-danger mb-3">
                      {error}
                    </div>
                  )}

                  {config.fields.map((f, idx) => (
                    <div className="mb-3" key={idx}>
                      <label className="form-label text-muted small fw-bold uppercase">
                        {f.label} {f.required && <span className="text-danger">*</span>}
                      </label>
                      {f.type === 'textarea' ? (
                        <textarea
                          name={f.name}
                          className="form-control form-glass-control"
                          rows="3"
                          value={formData[f.name] || ''}
                          onChange={handleInputChange}
                          required={f.required}
                        />
                      ) : (
                        <input
                          type={f.type}
                          name={f.name}
                          className="form-control form-glass-control"
                          value={formData[f.name] || ''}
                          onChange={handleInputChange}
                          required={f.required}
                        />
                      )}
                    </div>
                  ))}
                </div>
                <div className="modal-footer p-3 gap-2" style={{ borderTop: '1px solid var(--border-color)' }}>
                  <button type="button" className="btn btn-premium-outline btn-sm py-2 px-3" onClick={() => setShowModal(false)}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-premium-purple btn-sm py-2 px-4" disabled={saving}>
                    {saving ? <span className="spinner-border spinner-border-sm"></span> : 'Save'}
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

export default AdminMetadata;
