/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2024 The Bootstrap Authors, 2024-2025 Jonathan M. Lane.
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */
var bootstrap = (() => {
  'use strict'

  const getStoredTheme = () => localStorage.getItem('theme')
  const setStoredTheme = theme => localStorage.setItem('theme', theme)

  const getPreferredTheme = () => {
    const storedTheme = getStoredTheme()
    if (storedTheme) {
      return storedTheme
    }
    return 'auto'
  }

  const setTheme = (theme) => {
    if (theme === 'auto') {
      document.documentElement.setAttribute('data-bs-theme', (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'))
    } else {
      document.documentElement.setAttribute('data-bs-theme', theme)
    }
  }

  setTheme(window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')

  return {
    theme: {
      get: getPreferredTheme,
      set: (theme) => {
        setStoredTheme(theme)
        setTheme(theme)
      }
    }
  }
})()