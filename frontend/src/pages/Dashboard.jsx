import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import api, { API_HOST } from '../services/api';
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
  PieChart, Pie, Cell, Legend, AreaChart, Area
} from 'recharts';

const CHART_COLORS = ['#8247e5', '#00d4e8', '#39d353', '#fbbf24', '#fb923c', '#f87171', '#0d9488'];

const Dashboard = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  // Read computed CSS variables for chart styling (theme-aware)
  const getChartStyle = () => {
    const style = getComputedStyle(document.documentElement);
    return {
      grid:    style.getPropertyValue('--chart-grid').trim()    || '#242b3b',
      axis:    style.getPropertyValue('--chart-axis').trim()    || '#909bb0',
      bgTip:   style.getPropertyValue('--chart-tooltip-bg').trim()    || '#11151d',
      borTip:  style.getPropertyValue('--chart-tooltip-border').trim() || '#242b3b',
      textTip: style.getPropertyValue('--chart-label-color').trim()    || '#f1f3f7',
    };
  };

  const [chartStyle, setChartStyle] = useState(getChartStyle());

  useEffect(() => {
    const fetchStats = async () => {
      setLoading(true);
      try {
        const response = await api.get('/dashboard');
        setStats(response.data);
      } catch (err) {
        console.error('Failed to load dashboard stats', err);
      } finally {
        setLoading(false);
      }
    };
    fetchStats();

    // Re-compute chart colors when theme changes
    const observer = new MutationObserver(() => setChartStyle(getChartStyle()));
    observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });
    return () => observer.disconnect();
  }, []);

  if (loading) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border" style={{ color: 'var(--accent-purple)' }} role="status"></div>
      </div>
    );
  }

  if (!stats) {
    return <div className="text-theme-secondary p-4">No statistics data available.</div>;
  }

  const tooltipStyle = {
    background: chartStyle.bgTip,
    border: `1px solid ${chartStyle.borTip}`,
    borderRadius: '8px',
  };

  return (
    <div className="container-fluid py-2 fade-in">
      {/* Top row: KPIs */}
      <div className="row g-3 mb-4">
        <div className="col-6 col-md-4 col-xl">
          <div className="kpi-card">
            <div className="kpi-icon-wrapper kpi-purple">
              <i className="bi bi-controller"></i>
            </div>
            <div>
              <div className="kpi-card-label">Owned</div>
              <h3 className="mb-0 kpi-card-value display-font">{stats.totalGamesOwned}</h3>
            </div>
          </div>
        </div>
        
        <div className="col-6 col-md-4 col-xl">
          <div className="kpi-card">
            <div className="kpi-icon-wrapper kpi-green">
              <i className="bi bi-check-circle-fill"></i>
            </div>
            <div>
              <div className="kpi-card-label">Completed</div>
              <h3 className="mb-0 kpi-card-value display-font">{stats.totalCompletedGames}</h3>
            </div>
          </div>
        </div>

        <div className="col-6 col-md-4 col-xl">
          <div className="kpi-card" style={{ borderLeft: '3px solid var(--accent-teal)' }}>
            <div className="kpi-icon-wrapper kpi-teal">
              <i className="bi bi-play-circle-fill"></i>
            </div>
            <div>
              <div className="kpi-card-label">Unplayed</div>
              <h3 className="mb-0 kpi-card-value display-font">{stats.totalUnplayedGames}</h3>
            </div>
          </div>
        </div>

        <div className="col-6 col-md-6 col-xl">
          <div className="kpi-card">
            <div className="kpi-icon-wrapper kpi-info">
              <i className="bi bi-pc-display"></i>
            </div>
            <div>
              <div className="kpi-card-label">Platforms</div>
              <h3 className="mb-0 kpi-card-value display-font">{stats.totalPlatforms}</h3>
            </div>
          </div>
        </div>

        <div className="col-6 col-md-6 col-xl">
          <div className="kpi-card">
            <div className="kpi-icon-wrapper kpi-warning">
              <i className="bi bi-shop"></i>
            </div>
            <div>
              <div className="kpi-card-label">Services</div>
              <h3 className="mb-0 kpi-card-value display-font">{stats.totalServices}</h3>
            </div>
          </div>
        </div>
      </div>

      {/* Completion Percentage KPI */}
      <div className="row g-4 mb-4">
        <div className="col-12 col-xl-4">
          <div className="glass-panel p-4 h-100 d-flex flex-column justify-content-center align-items-center text-center">
            <h5 className="display-font mb-3">Completion Rate</h5>
            <div className="position-relative d-flex align-items-center justify-content-center" style={{ width: '160px', height: '160px' }}>
              <svg width="160" height="160" viewBox="0 0 160 160">
                <circle cx="80" cy="80" r="70" fill="none" stroke="var(--border-color)" strokeWidth="10" />
                <circle 
                  cx="80" cy="80" r="70" fill="none" 
                  stroke="var(--accent-purple)" strokeWidth="10" 
                  strokeDasharray="440" 
                  strokeDashoffset={440 - (440 * stats.completionPercentage) / 100}
                  strokeLinecap="round"
                  transform="rotate(-90 80 80)"
                  style={{ transition: 'stroke-dashoffset 1s ease' }}
                />
              </svg>
              <div className="position-absolute" style={{ color: 'var(--text-primary)' }}>
                <h2 className="mb-0 display-font fw-bold">{stats.completionPercentage}%</h2>
                <div className="text-muted small uppercase">Finished</div>
              </div>
            </div>
            <div className="mt-3 text-muted small">
              {stats.wishlistCount} Games on Wishlist | {stats.backlogCount} in Backlog Queue
            </div>
          </div>
        </div>

        {/* Top 3 Games list */}
        <div className="col-12 col-xl-8">
          <div className="glass-panel p-4 h-100">
            <h5 className="display-font mb-3 pb-2" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Most Played Games</h5>
            <div className="d-flex flex-column gap-3">
              {stats.mostPlayedGames?.length === 0 ? (
                <div className="text-muted small py-4 text-center">No gameplay logged yet. Start playing to track time!</div>
              ) : (
                stats.mostPlayedGames.map((game, idx) => (
                  <div key={game.id} className="d-flex align-items-center gap-3">
                    <span className="display-font fw-bold fs-4 text-theme-muted" style={{ width: '24px' }}>#{idx + 1}</span>
                    <Link to={`/games/${game.id}`} className="rounded overflow-hidden flex-shrink-0" style={{ width: '45px', height: '60px' }}>
                      <img 
                        src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                        alt={game.title} 
                        className="w-100 h-100 object-fit-cover"
                        onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                      />
                    </Link>
                    <div className="flex-grow-1 min-w-0">
                      <Link to={`/games/${game.id}`} className="text-decoration-none">
                        <h6 className="mb-0 text-truncate fw-bold" style={{ color: 'var(--text-primary)' }}>{game.title}</h6>
                      </Link>
                      <span className="text-muted small">{game.platforms.join(', ')}</span>
                    </div>
                    <div className="text-end">
                      <div className="fw-bold display-font" style={{ color: 'var(--accent-purple)' }}>{game.hoursPlayed} hrs</div>
                      <div className="text-muted small">Hours logged</div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Row: Charts visualizations */}
      <div className="row g-4 mb-4">
        {/* Platform chart */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4" style={{ height: '350px' }}>
            <h5 className="display-font mb-3">Games by Platform</h5>
            <div className="w-100 h-100" style={{ maxHeight: '265px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={stats.gamesByPlatform}>
                  <CartesianGrid strokeDasharray="3 3" stroke={chartStyle.grid} />
                  <XAxis dataKey="Name" stroke={chartStyle.axis} fontSize={12} tickLine={false} />
                  <YAxis stroke={chartStyle.axis} fontSize={12} tickLine={false} />
                  <Tooltip contentStyle={tooltipStyle} labelStyle={{ color: chartStyle.textTip }} />
                  <Bar dataKey="Value" fill="var(--accent-purple)" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Genre chart */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4" style={{ height: '350px' }}>
            <h5 className="display-font mb-3">Games by Genre</h5>
            <div className="w-100 h-100" style={{ maxHeight: '265px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={stats.gamesByGenre}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    outerRadius={80}
                    dataKey="Value"
                    nameKey="Name"
                    label={({ Name, percent }) => `${Name} (${(percent * 100).toFixed(0)}%)`}
                  >
                    {stats.gamesByGenre.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={CHART_COLORS[index % CHART_COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip contentStyle={tooltipStyle} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Release date Year trend */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4" style={{ height: '350px' }}>
            <h5 className="display-font mb-3">Games by Release Year</h5>
            <div className="w-100 h-100" style={{ maxHeight: '265px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={stats.gamesByReleaseYear}>
                  <CartesianGrid strokeDasharray="3 3" stroke={chartStyle.grid} />
                  <XAxis dataKey="Name" stroke={chartStyle.axis} fontSize={12} />
                  <YAxis stroke={chartStyle.axis} fontSize={12} />
                  <Tooltip contentStyle={tooltipStyle} />
                  <Area type="monotone" dataKey="Value" stroke="var(--accent-teal)" fill="var(--accent-teal-glow)" strokeWidth={2} />
                </AreaChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>

        {/* Play completion status distribution */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4" style={{ height: '350px' }}>
            <h5 className="display-font mb-3">Library Status Distribution</h5>
            <div className="w-100 h-100" style={{ maxHeight: '265px' }}>
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={stats.gamesByStatus}
                    cx="50%"
                    cy="50%"
                    innerRadius={50}
                    outerRadius={80}
                    dataKey="Value"
                    nameKey="Name"
                  >
                    {stats.gamesByStatus.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={CHART_COLORS[(index + 2) % CHART_COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip contentStyle={tooltipStyle} />
                  <Legend verticalAlign="bottom" height={36} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      </div>

      {/* Row: Recently added & recently completed */}
      <div className="row g-4">
        {/* Recently Added */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4">
            <h5 className="display-font mb-3 pb-2" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Recently Added</h5>
            <div className="d-flex flex-column gap-3">
              {stats.recentlyAddedGames?.length === 0 ? (
                <div className="text-muted small py-4 text-center">Your library is empty.</div>
              ) : (
                stats.recentlyAddedGames.map(game => (
                  <div key={game.id} className="d-flex align-items-center gap-3">
                    <Link to={`/games/${game.id}`} className="rounded overflow-hidden flex-shrink-0" style={{ width: '40px', height: '54px' }}>
                      <img 
                        src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                        alt={game.title} 
                        className="w-100 h-100 object-fit-cover"
                        onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                      />
                    </Link>
                    <div className="flex-grow-1 min-w-0">
                      <Link to={`/games/${game.id}`} className="text-decoration-none">
                        <h6 className="mb-0 text-truncate fw-bold" style={{ color: 'var(--text-primary)' }}>{game.title}</h6>
                      </Link>
                      <span className="text-muted small">{game.platforms.join(', ') || '-'}</span>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

        {/* Recently Completed */}
        <div className="col-12 col-md-6">
          <div className="glass-panel p-4">
            <h5 className="display-font mb-3 pb-2" style={{ borderBottom: '1px solid var(--border-subtle)' }}>Recently Completed</h5>
            <div className="d-flex flex-column gap-3">
              {stats.recentlyCompletedGames?.length === 0 ? (
                <div className="text-muted small py-4 text-center">No completed games logged yet. Finish a game to show it here!</div>
              ) : (
                stats.recentlyCompletedGames.map(game => (
                  <div key={game.id} className="d-flex align-items-center gap-3">
                    <Link to={`/games/${game.id}`} className="rounded overflow-hidden flex-shrink-0" style={{ width: '40px', height: '54px' }}>
                      <img 
                        src={game.coverImage ? `${API_HOST}${game.coverImage}` : 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'} 
                        alt={game.title} 
                        className="w-100 h-100 object-fit-cover"
                        onError={(e) => {e.target.src = 'https://placehold.co/300x400/1b202c/909bb0?text=No+Cover'}}
                      />
                    </Link>
                    <div className="flex-grow-1 min-w-0">
                      <Link to={`/games/${game.id}`} className="text-decoration-none">
                        <h6 className="mb-0 text-truncate fw-bold" style={{ color: 'var(--text-primary)' }}>{game.title}</h6>
                      </Link>
                      <span className="text-muted small">{game.platforms.join(', ') || '-'}</span>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>
      </div>

    </div>
  );
};

export default Dashboard;
