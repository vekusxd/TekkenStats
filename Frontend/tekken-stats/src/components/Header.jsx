import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useInlineSearch } from '../hooks/useInlineSearch';
import styles from '../styles/TekkenStatsApp.module.css';

const Header = () => {
  const navigate = useNavigate();
  const {
    searchQuery,
    inputError,
    suggestions,
    showSuggestions,
    loading,
    searchPlayers,
    handleInputChange,
    setSearchQuery,
    setShowSuggestions,
    setInputError
  } = useInlineSearch();

  const handleSearch = async (e) => {
    e.preventDefault();
    try {
      const results = await searchPlayers(searchQuery);
      if (results.length > 0) {
        navigate(`/player/${results[0].tekkenId}`);
        setSearchQuery('');
      }
    } catch (error) {
      setInputError(error.message);
    }
  };

  const handlePlayerClick = (player) => {
    setSearchQuery('');
    navigate(`/player/${player.tekkenId}`);
    setShowSuggestions(false);
  };

  return (
    <header className={styles.header}>
      <div className={styles.headerContainer}>
        <div className={styles.headerContent}>
          <Link to="/" className={styles.logo}>Tekken Stats</Link>
          <div className={styles.headerSearchContainer}>
            <form onSubmit={handleSearch} className={styles.headerSearchForm}>
              <div className={styles.headerInputWrapper}>
                <input 
                  type="text" 
                  placeholder="Search player..." 
                  className={`${styles.headerSearchInput} ${inputError ? styles.inputError : ''}`}
                  value={searchQuery}
                  onChange={(e) => handleInputChange(e.target.value)}
                  onFocus={() => searchQuery.length > 0 && setShowSuggestions(true)}
                  onBlur={() => setTimeout(() => setShowSuggestions(false), 200)}
                />
                {inputError && (
                  <div className={styles.headerError}>
                    {inputError}
                  </div>
                )}
                {showSuggestions && suggestions.length > 0 && (
                  <div className={styles.headerSuggestionsDropdown}>
                    {suggestions.map(player => (
                      <div 
                        key={player.tekkenId}
                        className={styles.headerSuggestionItem}
                        onClick={() => handlePlayerClick(player)}
                      >
                        {player.name}
                      </div>
                    ))}
                  </div>
                )}
              </div>
              <button 
                type="submit" 
                className={styles.headerSearchIcon}
                disabled={loading || !searchQuery.trim()}
              >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                </svg>
              </button>
            </form>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;