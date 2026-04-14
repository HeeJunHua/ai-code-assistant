import { useState } from 'react'
import { aiService, ProcessResponse } from './services/api'
import { useTheme } from './context/ThemeContext'
import { ThemeToggle } from './components/ThemeToggle'

type ActionType = 'explain' | 'fix' | 'optimize'

function App() {
  const [code, setCode] = useState('')
  const [result, setResult] = useState<ProcessResponse | null>(null)
  const [loadingAction, setLoadingAction] = useState<ActionType | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [copied, setCopied] = useState(false)
  const { theme } = useTheme()
  const MAX_CHARS = 5000


  const handleProcess = async (action: ActionType) => {
    if (!code.trim()) {
      setError('Please enter some code to process')
      return
    }

    setLoadingAction(action)
    setError(null)
    setResult(null)
    setCopied(false)

    if (code.length > MAX_CHARS) {
      setError(`Code too large. Max ${MAX_CHARS} characters allowed.`)
      return
    }
    
    try {
      const response = await aiService.processCode({ code, action })
      setResult(response)
    } catch (err) {
      setError('Failed to process code. Please check if Ollama is running on localhost:11434.')
      console.error('API Error:', err)
    } finally {
      setLoadingAction(null)
    }
  }

  const handleCopy = async () => {
    if (result?.result) {
      try {
        await navigator.clipboard.writeText(result.result)
        setCopied(true)
        setTimeout(() => setCopied(false), 2000)
      } catch (err) {
        console.error('Failed to copy:', err)
      }
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900 transition-colors duration-300">
      <ThemeToggle />
      
      {/* Header */}
      <div className="container mx-auto px-4 py-8 max-w-6xl">
        <div className="text-center mb-8">
          <h1 className="text-4xl md:text-5xl font-bold bg-gradient-to-r from-slate-900 via-blue-900 to-indigo-900 dark:from-white dark:via-blue-200 dark:to-indigo-200 bg-clip-text text-transparent mb-4">
            AI Code Assistant
          </h1>
          <p className="text-slate-600 dark:text-slate-300 text-lg md:text-xl max-w-2xl mx-auto leading-relaxed">
            Paste your code and let AI help you understand, fix, or optimize it with powerful local processing
          </p>
        </div>

        {/* Main Content */}
        <div className="grid lg:grid-cols-2 gap-8">
          {/* Input Section */}
          <div className="bg-white dark:bg-slate-800 rounded-2xl shadow-xl border border-slate-200 dark:border-slate-700 p-6">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-3 h-3 bg-green-400 rounded-full animate-pulse"></div>
              <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-200">Input Code</h2>
            </div>
            
            <div className="relative">
              <textarea
                value={code}
                onChange={(e) => {
                  if (e.target.value.length <= MAX_CHARS) {
                    setCode(e.target.value)
                  }
                }}
                maxLength={MAX_CHARS}
                placeholder="Paste your code here... (JavaScript, Python, C#, Java, etc.)"
                className="w-full h-80 p-4 bg-slate-50 dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-xl text-slate-800 dark:text-slate-200 font-mono text-sm resize-none focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 placeholder-slate-400 dark:placeholder-slate-500"
              />
              <div className="absolute bottom-3 right-3 text-xs text-slate-400 dark:text-slate-500">
                {code.length} characters
              </div>
            </div>
            
            <div className="text-xs text-slate-400 dark:text-slate-500 mt-1">
              {code.length} / {MAX_CHARS} characters
            </div>
            {/* Action Buttons */}
            <div className="mt-6 flex flex-wrap gap-3">
              <button
                onClick={() => handleProcess('explain')}
                disabled={loadingAction !== null}
                className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 disabled:from-slate-400 disabled:to-slate-500 disabled:cursor-not-allowed text-white font-medium rounded-xl transition-all duration-200 transform hover:scale-105 active:scale-95 shadow-lg"
              >
                {loadingAction === 'explain' ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    Explaining...
                  </>
                ) : (
                  <>
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    Explain
                  </>
                )}
              </button>

              <button
                onClick={() => handleProcess('fix')}
                disabled={loadingAction !== null}
                className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 disabled:from-slate-400 disabled:to-slate-500 disabled:cursor-not-allowed text-white font-medium rounded-xl transition-all duration-200 transform hover:scale-105 active:scale-95 shadow-lg"
              >
                {loadingAction === 'fix' ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    Fixing...
                  </>
                ) : (
                  <>
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M14.7 6.3a1 1 0 000 1.4l1.6 1.6a1 1 0 001.4 0l3.77-3.77a6 6 0 01-7.94 7.94l-6.91 6.91a2.12 2.12 0 01-3-3l6.91-6.91a6 6 0 017.94-7.94l-3.76 3.76z" />
                    </svg>
                    Fix
                  </>
                )}
              </button>

              <button
                onClick={() => handleProcess('optimize')}
                disabled={loadingAction !== null}
                className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-purple-600 to-purple-700 hover:from-purple-700 hover:to-purple-800 disabled:from-slate-400 disabled:to-slate-500 disabled:cursor-not-allowed text-white font-medium rounded-xl transition-all duration-200 transform hover:scale-105 active:scale-95 shadow-lg"
              >
                {loadingAction === 'optimize' ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    Optimizing...
                  </>
                ) : (
                  <>
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                    </svg>
                    Optimize
                  </>
                )}
              </button>
            </div>

            {/* Error Display */}
            {error && (
              <div className="mt-4 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl">
                <div className="flex items-center gap-3">
                  <svg className="w-5 h-5 text-red-500 dark:text-red-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <div className="text-red-700 dark:text-red-300 text-sm">{error}</div>
                </div>
              </div>
            )}
          </div>

          {/* Output Section */}
          <div className="bg-white dark:bg-slate-800 rounded-2xl shadow-xl border border-slate-200 dark:border-slate-700 p-6">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="w-3 h-3 bg-blue-400 rounded-full animate-pulse"></div>
                <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-200">AI Output</h2>
              </div>
              {result && (
                <button
                  onClick={handleCopy}
                  className="flex items-center gap-2 px-4 py-2 bg-slate-100 dark:bg-slate-700 hover:bg-slate-200 dark:hover:bg-slate-600 text-slate-700 dark:text-slate-200 rounded-lg transition-colors duration-200"
                >
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                  </svg>
                  {copied ? 'Copied!' : 'Copy'}
                </button>
              )}
            </div>

            {loadingAction && !result && (
              <div className="h-80 flex items-center justify-center">
                <div className="text-center">
                  <div className="inline-flex items-center gap-3 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-2xl p-6">
                    <div className="w-8 h-8 border-4 border-blue-200 dark:border-blue-700 border-t-blue-600 rounded-full animate-spin"></div>
                    <div className="text-blue-700 dark:text-blue-300 font-medium">Analyzing your code...</div>
                  </div>
                </div>
              </div>
            )}

            {!loadingAction && !result && !error && (
              <div className="h-80 flex items-center justify-center">
                <div className="text-center text-slate-500 dark:text-slate-400">
                  <svg className="w-16 h-16 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                  <p className="text-slate-600 dark:text-slate-300">Your AI-processed result will appear here</p>
                  <p className="text-sm text-slate-400 dark:text-slate-500 mt-2">Paste some code and click an action button</p>
                </div>
              </div>
            )}

            {result && (
              <div className="h-80 overflow-auto">
                <div className="mb-4 flex items-center gap-3">
                  <span className="inline-flex items-center px-3 py-1 bg-gradient-to-r from-blue-100 to-blue-200 dark:from-blue-900/30 dark:to-blue-800/30 text-blue-800 dark:text-blue-200 text-sm font-medium rounded-full">
                    <div className="w-2 h-2 bg-blue-500 rounded-full mr-2"></div>
                    {result.action}
                  </span>
                  <span className="text-xs text-slate-500 dark:text-slate-400 bg-slate-100 dark:bg-slate-700 px-2 py-1 rounded-full">
                    Generated {new Date(result.timestamp).toLocaleString()}
                  </span>
                </div>
                
                <div className="bg-slate-50 dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-xl p-4">
                  <pre className="whitespace-pre-wrap text-slate-800 dark:text-slate-200 font-mono text-sm leading-relaxed">
                    {result.result}
                  </pre>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Footer */}
        <div className="mt-12 text-center">
          <div className="inline-flex items-center gap-2 px-6 py-3 bg-white dark:bg-slate-800 rounded-full shadow-lg border border-slate-200 dark:border-slate-700">
            <div className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
            <span className="text-sm text-slate-600 dark:text-slate-300 font-medium">
              Local AI processing with Ollama • No data sent to external servers
            </span>
          </div>
        </div>
      </div>
    </div>
  )
}

export default App