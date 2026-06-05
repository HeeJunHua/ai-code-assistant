import React, { useState } from 'react'
import { aiService, ProcessResponse } from '../services/api'

/**
 * Inline configuration UI for custom AI provider.
 * Allows user to enter endpoint, API key, and model name.
 * The API key is stored only in component state (memory) and cleared on page reload.
 */
interface ModelConfigProps {
  config: {
    endpoint: string
    apiKey: string
    model: string
  }
  setConfig: React.Dispatch<
    React.SetStateAction<{ endpoint: string; apiKey: string; model: string }>
  >
}

const ModelConfig: React.FC<ModelConfigProps> = ({ config, setConfig }) => {
  const [testStatus, setTestStatus] = useState<string>('')

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setConfig((prev) => ({ ...prev, [name]: value }))
  }

  const testConnection = async () => {
    setTestStatus('Testing...')
    try {
      // Send a minimal request to verify the custom provider works.
      const testReq = {
        code: "// test connection",
        action: "explain" as const,
        endpoint: config.endpoint,
        apiKey: config.apiKey,
        model: config.model,
      }
      const response = await aiService.processCode(testReq)
      setTestStatus('Success! Received response.')
    } catch (err: any) {
      setTestStatus(`Error: ${err?.message ?? String(err)}`)
    }
  }

  const resetToOllama = () => {
    setConfig({ endpoint: '', apiKey: '', model: '' })
    setTestStatus('')
  }

  return (
    <div className="bg-gray-50 dark:bg-slate-800 p-4 rounded-xl mb-6 border border-gray-200 dark:border-slate-700">
      <h2 className="text-lg font-semibold text-slate-800 dark:text-slate-200 mb-3">
        Custom AI Provider Configuration
      </h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
        <div>
          <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            Endpoint URL
          </label>
          <input
            type="text"
            name="endpoint"
            value={config.endpoint}
            onChange={handleChange}
            placeholder="https://api.openai.com/v1/chat/completions"
            className="w-full rounded-md border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-slate-100 focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            API Key
          </label>
          <input
            type="password"
            name="apiKey"
            value={config.apiKey}
            onChange={handleChange}
            placeholder="sk-..."
            className="w-full rounded-md border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-slate-100 focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
            Model Name
          </label>
          <input
            type="text"
            name="model"
            value={config.model}
            onChange={handleChange}
            placeholder="gpt-3.5-turbo"
            className="w-full rounded-md border-gray-300 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-slate-100 focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>
      <div className="flex items-center space-x-4">
        <button
          onClick={testConnection}
          className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
        >
          Test Connection
        </button>
        <button
          onClick={resetToOllama}
          className="px-4 py-2 bg-gray-300 dark:bg-slate-600 text-slate-800 dark:text-slate-100 rounded-md hover:bg-gray-400"
        >
          Reset to Ollama
        </button>
        {testStatus && (
          <span className="text-sm text-slate-700 dark:text-slate-300 ml-2">{testStatus}</span>
        )}
      </div>
    </div>
  )
}

export default ModelConfig
