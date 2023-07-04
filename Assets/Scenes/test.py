#import serial
import numpy as np
from sklearn.cross_decomposition import CCA

def getReferenceSignals(length, target_freq, samplingRate):
		# generate sinusoidal reference templates for CCA for the first and second harmonics
		# type(int length), type(float target_freq)
		reference_signals = [] # reference_signals type: np.ndarray
		t = np.arange(0, (length/(samplingRate)), step=1.0/(samplingRate))
		#First harmonics/Fundamental freqeuncy
		reference_signals.append(np.sin(np.pi*2*target_freq*t))
		reference_signals.append(np.cos(np.pi*2*target_freq*t))
		#Second harmonics
		reference_signals.append(np.sin(np.pi*4*target_freq*t))
		reference_signals.append(np.cos(np.pi*4*target_freq*t)) 
		reference_signals = np.array(reference_signals)
		reference_signals = reference_signals.tolist() # reference_signals type: list
		return reference_signals
			
def getSignals(n_components, buffer, freq):
		# Perform Canonical correlation analysis (CCA)
		# buffer - consists of the EEG
		# freq - set of sinusoidal reference templates corresponding to the flicker frequency
		# type(int n_components), type(list buffer), type(list freq)
		cca = CCA(n_components)
		corr = np.zeros(n_components)
		numpyBuffer = np.array(buffer)
		freq = np.array(freq) # freq type: np.ndarray
		result = np.zeros((freq.shape)[0]) # result type: np.ndarray
		for freqIdx in range(0,(freq.shape)[0]):
			cca.fit(numpyBuffer.T,np.squeeze(freq[freqIdx,:,:]).T)
			O1_a,O1_b = cca.transform(numpyBuffer.T, np.squeeze(freq[freqIdx,:,:]).T)
			indVal = 0
			for indVal in range(0,n_components):
				corr[indVal] = np.corrcoef(O1_a[:,indVal],O1_b[:,indVal])[0,1]
			result[freqIdx] = np.max(corr)
		result = result.tolist() # result type: list
		return result

def getPythonFunc():
	return 3